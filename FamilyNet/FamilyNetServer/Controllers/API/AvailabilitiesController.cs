using System;
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
    public class AvailabilitiesController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;
        //private readonly IAvailabilityValidator _availabilityValidator;
        //private readonly IFilterConditionsAvailability _filterConditions;

        #endregion

        public AvailabilitiesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Volunteer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]AvailabilityDTO availabilityDTO)
        {
            //if (!_availabilityValidator.IsValid(availabilityDTO))
            //{
            //    return BadRequest();
            //}

            var availability = new Availability()
            {
                VolunteerID = User.FindFirst(ClaimTypes.Name)?.Value,
                Date = availabilityDTO.Date,
                VolunteerHours = availabilityDTO.VolunteerHours,
                FromHour = availabilityDTO.FromHour
            };

            await _unitOfWork.Availabilities.Create(availability);
            _unitOfWork.SaveChangesAsync();

            //availabilityDTO.ID = representative.ID;

            return Created("api/v1/{controller}/" + availability.ID, availabilityDTO);
        }
    }
}