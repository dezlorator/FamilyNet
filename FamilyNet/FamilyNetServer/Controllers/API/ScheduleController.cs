﻿using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using FamilyNetServer.Uploaders;
using DataTransferObjects;
using FamilyNetServer.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FamilyNetServer.Controllers.API
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;
        //private readonly IAvailabilityValidator _availabilityValidator;
        //private readonly IFilterConditionsAvailability _filterConditions;

        #endregion

        public ScheduleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Volunteer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAll(DateTime date)
        {
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var volunteerId = _unitOfWork.UserManager.FindByIdAsync(userId)
                            .Result.PersonID;

            var availabilities = _unitOfWork.Availabilities.GetAll()
                .Where(a => a.VolunteerID == volunteerId && a.Date > DateTime.Now)
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
                VolunteerHours = a.FreeHours,
            });

            return Ok(availabilitiesDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id)
        {
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
                VolunteerHours = availability.FreeHours
            };

            return Ok(availabilityDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Volunteer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]AvailabilityDTO availabilityDTO)
        {
            //TODO Validate Availability
            //BadRequest
            var diff = adjustDate(availabilityDTO);   

            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var volunteerId = _unitOfWork.UserManager.FindByIdAsync(userId)
                            .Result.PersonID;


            var availability = new Availability()
            {
                VolunteerID = volunteerId.Value,
                FreeHours = availabilityDTO.VolunteerHours,
                Date = availabilityDTO.StartTime.AddDays(diff)
            };
        
            await _unitOfWork.Availabilities.Create(availability);
            _unitOfWork.SaveChangesAsync();

            return Created("api/v1/{controller}/" + availability.ID, availabilityDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Volunteer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]AvailabilityDTO availabilityDTO)
        {
            //if (!_representativeValidator.IsValid(availabilityDTO))
            //{
            //    return BadRequest();
            //}

            var availability = await _unitOfWork.Availabilities.GetById(availabilityDTO.ID);

            if (availability == null)
            {
                return BadRequest();
            }
            var diff = adjustDate(availabilityDTO);

            availability.Date = availabilityDTO.StartTime.AddDays(diff);
            availability.FreeHours = availabilityDTO.VolunteerHours;

            _unitOfWork.Availabilities.Update(availability);
            _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Volunteer")]
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
    }
}