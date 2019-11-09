using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DataTransferObjects.Enums;
using FamilyNetServer.Models.Identity;
using System.Collections.Generic;
using FamilyNetServer.Validators;

namespace FamilyNetServer.Controllers.API
{
    //[Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAvailabilityValidator _validator;
        //private readonly IFilterConditionsAvailability _filterConditions;

        #endregion

        public ScheduleController(IUnitOfWork unitOfWork, IAvailabilityValidator availabilityValidator)
        {
            _validator = availabilityValidator;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAll(DateTime date)
        {
            var user = identify();

            var availabilities = _unitOfWork.Availabilities.GetAll()
                .Where(a => a.PersonID == user.PersonID && a.Date > DateTime.Now)
                .OrderBy(a => a.Date);

            if (availabilities == null)
            {
                return NotFound();
            }
            var availabilitiesDTO = availabilities.Select(a =>
            new AvailabilityDTO()
            {
                ID = a.ID,
                StartTime = a.Date,
                DayOfWeek = a.Date.DayOfWeek,
                FreeHours = a.FreeHours,
                Role = user.PersonType,
                IsReserved = a.IsReserved
            });

            return Ok(availabilitiesDTO);
        }

        #region поиск по дате квеста и продолжительности

        [HttpGet("freePersons")]
        //[Authorize(Roles = "Admin, CharityMaker, Representative")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetFreePersonIDsList(DateTime date, TimeSpan duration, PersonType role)
        {
            var personIdsList = _unitOfWork.Availabilities.Get(a =>
               date >= a.Date &&
               date < (a.Date + a.FreeHours) &&
               duration <= a.FreeHours &&
               a.Role == role &&
               a.IsReserved == false)
               .Select(a => new { personID = a.PersonID, availabilityID = a.ID }).GroupBy(a => a.personID);

            if (personIdsList == null)
            {
                return NotFound();
            }

            return Ok(personIdsList);
        }

        #endregion 

        //Reserve(AvailabilityID id) ? deReserve()

        #region сравнение расписаний волонтера и мецената

        [HttpGet("posibleDates")]
        [Authorize(Roles = "Admin, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult FindPossibleDate()
        {
            var user = identify();

            var myAvailabilities = _unitOfWork.Availabilities.GetAll()
                .Where(a => a.PersonID == user.PersonID &&
                a.Role == user.PersonType &&
                a.Date > DateTime.Now)
                .OrderBy(a => a.Date);

            IEnumerable<Availability> posibleDates = new List<Availability>();

            foreach (var my in myAvailabilities)
            {
                posibleDates.Concat(_unitOfWork.Availabilities.Get(
                    a => a.Date <= my.Date &&
                    my.Date < (a.Date + a.FreeHours) &&
                    a.Role == PersonType.Volunteer &&
                    a.IsReserved == false));
            }

            if (posibleDates.Count() == 0)
            {
                return NotFound();
            }

            return Ok(posibleDates);
        }

        #endregion

        //reserve Availability client side controller => HttpPut



        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {
            var user = identify();

            var availability = await _unitOfWork.Availabilities.GetById(id);

            if (availability == null)
            {
                return BadRequest();
            }

            var availabilityDTO = new AvailabilityDTO()
            {
                ID = availability.ID,
                StartTime = availability.Date,
                DayOfWeek = availability.Date.DayOfWeek,
                FreeHours = availability.FreeHours,
                Role = user.PersonType,
                IsReserved = availability.IsReserved
            };

            return Ok(availabilityDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]AvailabilityDTO availabilityDTO)
        {
            var diff = adjustDate(availabilityDTO);

            availabilityDTO.StartTime = availabilityDTO.StartTime.AddDays(diff);

            if (!_validator.IsValid(availabilityDTO))
            {
                return BadRequest();
            }

            var overlaps = _unitOfWork.Availabilities
                .Get(a => _validator
                .IsOverlaping(availabilityDTO, a)).Count();

            if (overlaps > 0)
            {
                //TODO logg
                return Conflict();
            }
     
            var user = identify();

            //var diff = adjustDate(dto);

            var availability = new Availability()
            {
                PersonID = user.PersonID.Value,
                FreeHours = availabilityDTO.FreeHours,
                Date = availabilityDTO.StartTime,
                Role = user.PersonType,
            };

            await _unitOfWork.Availabilities.Create(availability);
            _unitOfWork.SaveChangesAsync();

            return Created("api/v1/{controller}/" + availability.ID, availabilityDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]AvailabilityDTO availabilityDTO)
        {
            var diff = adjustDate(availabilityDTO);

            availabilityDTO.StartTime = availabilityDTO.StartTime.AddDays(diff);

            if (!_validator.IsValid(availabilityDTO))
            {
                return BadRequest();
            }

            var overlaps = _unitOfWork.Availabilities.Get(a => _validator
                .IsOverlaping(availabilityDTO, a) && 
                a.ID != availabilityDTO.ID).Count();

            if (overlaps > 0)
            {
                //TODO logg
                return Conflict();
            }

            var availability = await _unitOfWork.Availabilities.GetById(availabilityDTO.ID);

            if (availability == null)
            {
                return BadRequest();
            }

            availability.Date = availabilityDTO.StartTime;
            availability.FreeHours = availabilityDTO.FreeHours;

            _unitOfWork.Availabilities.Update(availability);
            _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var availability = await _unitOfWork.Availabilities.GetById(id);

            if (availability == null)
            {
                return BadRequest();
            }

            await _unitOfWork.Availabilities.HardDelete(id);
            _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        private double adjustDate(AvailabilityDTO availabilityDTO)
        {
            var diff = (double)availabilityDTO.DayOfWeek - (double)DateTime.Now.DayOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }

            if (diff == 0)
            {
                var timeDiff = availabilityDTO.StartTime.TimeOfDay < DateTime.Now.TimeOfDay;
                diff += (timeDiff) ? 7 : 0;
            }

            return diff;
        }

        private ApplicationUser identify()
        {
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            return _unitOfWork.UserManager.FindByIdAsync(userId).Result;
        }
    }
}