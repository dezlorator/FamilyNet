using System;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Models.Identity;
using FamilyNetServer.Validators;
using FamilyNetServer.HttpHandlers;
using DataTransferObjects;
using DataTransferObjects.Enums;
using Newtonsoft.Json;
using FamilyNetServer.Helpers;

namespace FamilyNetServer.Controllers.API
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAvailabilityValidator _validator;
        private readonly ILogger<ScheduleController> _logger;
        private readonly IIdentityExtractor _identityExtractor;
        private readonly IScheduleHelper _helper;

        #endregion

        #region ctors

        public ScheduleController(IUnitOfWork unitOfWork,
            IAvailabilityValidator availabilityValidator,
            ILogger<ScheduleController> logger, 
            IIdentityExtractor identityExtractor,
            IScheduleHelper helper)
        {
            _identityExtractor = identityExtractor;
            _validator = availabilityValidator;
            _unitOfWork = unitOfWork;
            _helper = helper;
            _logger = logger;
        }

        #endregion

        [HttpGet]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAll()
        {
            _logger.LogInformation("{info}",
               "Endpoint Schedule/api/v1 GetAll was called");

            var user = identify();

            var availabilities = _unitOfWork.Availabilities.GetAll()
                .Where(a => a.PersonID == user.PersonID
                    && a.Date > DateTime.Now && !a.IsDeleted)
                .OrderBy(a => a.Date);

            if (availabilities.Count() == 0)
            {
                _logger.LogInformation("{status}{info}",
                   StatusCodes.Status404NotFound,
                   "List of Availabilities is empty");

                return Ok(new List<AvailabilityDTO>().AsQueryable());
            }
            var availabilitiesDTO = availabilities.Select(a =>
            new AvailabilityDTO()
            {
                ID = a.ID,
                PersonID = a.PersonID,
                StartTime = a.Date,
                DayOfWeek = a.Date.DayOfWeek,
                FreeHours = a.FreeHours,
                Role = user.PersonType,
                IsReserved = a.IsReserved,
                QuestID = a.QuestID,
                QuestName = a.QuestName
            });

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(availabilitiesDTO));

            return Ok(availabilitiesDTO);
        }

        [HttpGet("freePersons")]
        [Authorize(Roles = "Admin, CharityMaker, Representative")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetFreePersonIDsList(DateTime date, TimeSpan duration,
                                                                 PersonType role)
        {
            _logger.LogInformation("{info}",
               "Endpoint Schedule/api/v1/freePersons" +
               $" GetFreePersonIDsList({date}, {duration}, {role}) was called");
            var personIdsList = _unitOfWork.Availabilities.Get(a =>
                   (date >= a.Date) && (date < (a.Date + a.FreeHours))
                   && (duration <= a.FreeHours) && (a.Role == role)
                   && !a.IsReserved && !a.IsDeleted)
               .Select(a => new { personID = a.PersonID, availabilityID = a.ID }).GroupBy(a => a.personID);

            if (personIdsList.Count() == 0)
            {
                _logger.LogError("{info}{status}",
                    $"Persons wasn't found",
                    StatusCodes.Status404NotFound);

                return NotFound();
            }

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(personIdsList));

            return Ok(personIdsList);
        }

        [HttpGet("switchReserveStatus/{id}")]
        [Authorize(Roles = "Admin, Volunteer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SwitchReserveStatusById(int id,
            [FromQuery]string questName, [FromQuery] int questID)
        {
            _logger.LogInformation("{info}",
                "Endpoint Schedule/api/v1/switchReserveStatus" +
                $" SwitchReserveStatusById({id}, {questName}, {questID}) was called");

            var availability = await _unitOfWork.Availabilities.GetById(id);

            if (availability == null)
            {
                _logger.LogError("{info}{status}",
                   $"Availability wasn't found [id:{id}]",
                   StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            if (!availability.IsReserved)
            {
                availability.IsReserved = !availability.IsReserved;
                availability.QuestID = questID;
                availability.QuestName = questName;
            }
            else
            {
                availability.IsReserved = !availability.IsReserved;
                availability.QuestID = 0;
                availability.QuestName = null;
            }

            _unitOfWork.Availabilities.Update(availability);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(availability));

            return Ok();
        }

        [HttpGet("posibleDates")]
        [Authorize(Roles = "Admin, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult FindPossibleDate()
        {
            _logger.LogInformation("{info}",
                "Endpoint Schedule/api/v1/posibleDates" +
                $" FindPossibleDate() was called");

            var user = identify();

            var myAvailabilities = _unitOfWork.Availabilities.GetAll()
                .Where(a =>(a.PersonID == user.PersonID)
                    && (a.Role == user.PersonType) && (a.Date > DateTime.Now)
                    && !a.IsDeleted)
                .OrderBy(a => a.Date);

            if (myAvailabilities.Count() == 0)
            {
                _logger.LogError("{info}{status}",
                    $"Availabilities for {user.PersonType} wasn't found",
                    StatusCodes.Status404NotFound);

                return NotFound();
            }

            IEnumerable<Availability> matchingAvailabilities = new List<Availability>();

            foreach (var my in myAvailabilities)
            {
                matchingAvailabilities.Concat(_unitOfWork.Availabilities.Get(
                    a => (a.Date <= my.Date)
                    && my.Date < (a.Date + a.FreeHours)
                    && (a.Role == PersonType.Volunteer) 
                    && !a.IsReserved && !a.IsDeleted));
            }

            if (matchingAvailabilities.Count() == 0)
            {
                _logger.LogError("{info}{status}",
                    $"Matching availabilities wasn't found",
                    StatusCodes.Status404NotFound);

                return NotFound();
            }

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(matchingAvailabilities));

            return Ok(matchingAvailabilities);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("{info}",
                $"Endpoint Schedule/api/v1 GetById({id}) was called");

            var user = identify();

            var availability = await _unitOfWork.Availabilities.GetById(id);

            if (availability == null)
            {
                _logger.LogError("{info}{status}",
                    $"Availabilities wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var availabilityDTO = new AvailabilityDTO()
            {
                ID = availability.ID,
                PersonID = availability.PersonID,
                StartTime = availability.Date,
                DayOfWeek = availability.Date.DayOfWeek,
                FreeHours = availability.FreeHours,
                Role = user.PersonType,
                IsReserved = availability.IsReserved,
                QuestID = availability.QuestID,
                QuestName = availability.QuestName
            };

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(availabilityDTO));

            return Ok(availabilityDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]AvailabilityDTO availabilityDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Schedule/api/v1 [POST] was called", userId, token);

            var diff = _helper.AdjustDate(availabilityDTO);

            availabilityDTO.StartTime = availabilityDTO.StartTime.AddDays(diff);

            if (!_validator.IsValid(availabilityDTO))
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                    StatusCodes.Status400BadRequest, token, userId,
                    "AvailabilityDTO is invalid");

                return BadRequest();
            }

            var overlaps = _unitOfWork.Availabilities
                .Get(a => a.IsDeleted == false && _validator
                .IsOverlaping(availabilityDTO, a)).Count();

            if (overlaps > 0)
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                    StatusCodes.Status409Conflict, token, userId,
                    "Time overlap was found");

                return Conflict();
            }

            var user = identify();

            var availability = new Availability()
            {
                PersonID = user.PersonID.Value,
                FreeHours = availabilityDTO.FreeHours,
                Date = availabilityDTO.StartTime,
                Role = user.PersonType,
            };

            await _unitOfWork.Availabilities.Create(availability);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Representative was saved [id:{availability.ID}]");

            return Created("api/v1/{controller}/" + availability.ID, availabilityDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]AvailabilityDTO availabilityDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Schedule/api/v1 [PUT] was called", userId, token);

            var diff = _helper.AdjustDate(availabilityDTO);

            availabilityDTO.StartTime = availabilityDTO.StartTime.AddDays(diff);

            if (!_validator.IsValid(availabilityDTO))
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                    StatusCodes.Status400BadRequest, token, userId,
                    "AvailabilityDTO is invalid");

                return BadRequest();
            }

            var overlaps = _unitOfWork.Availabilities.Get(a => _validator
                .IsOverlaping(availabilityDTO, a) &&
                a.ID != availabilityDTO.ID).Count();

            if (overlaps > 0)
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                   StatusCodes.Status409Conflict, token, userId,
                   "Time overlap was found");

                return Conflict();
            }

            var availability = await _unitOfWork.Availabilities.GetById(availabilityDTO.ID);

            if (availability == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Availability was not found [id:{availabilityDTO.ID}]", userId, token);

                return BadRequest();
            }

            availability.Date = availabilityDTO.StartTime;
            availability.FreeHours = availabilityDTO.FreeHours;

            _unitOfWork.Availabilities.Update(availability);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                 token, userId, StatusCodes.Status204NoContent,
                 $"Availability was updated [id:{availability.ID}]");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
               "Endpoint Schedule/api/v1 [DELETE] was called", userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            var availability = await _unitOfWork.Availabilities.GetById(id);

            if (availability == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Availability was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            availability.IsDeleted = true;

            _unitOfWork.Availabilities.Update(availability);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"Availability.IsDelete was updated [id:{id}]",
                userId, token);

            return Ok();
        }

        #region private methods

        private ApplicationUser identify()
        {
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            return _unitOfWork.UserManager.FindByIdAsync(userId).Result;
        }

        #endregion
    }
}