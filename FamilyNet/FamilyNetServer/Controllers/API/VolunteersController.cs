using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;
using Microsoft.Extensions.Options;
using FamilyNetServer.Configuration;
using FamilyNetServer.Filters.FilterParameters;
using Microsoft.AspNetCore.Authorization;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VolunteersController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploader _fileUploader;
        private readonly IVolunteerValidator _volunteerValidator;
        private readonly IFilterConditionsVolunteers _filterConditions;
        private readonly IOptionsSnapshot<ServerURLSettings> _settings;

        #endregion

        #region ctor

        public VolunteersController(IFileUploader fileUploader,
                                  IUnitOfWork unitOfWork,
                                  IVolunteerValidator volunteerValidator,
                                  IFilterConditionsVolunteers filterConditions,
                                  IOptionsSnapshot<ServerURLSettings> setings)
        {
            _fileUploader = fileUploader;
            _unitOfWork = unitOfWork;
            _volunteerValidator = volunteerValidator;
            _filterConditions = filterConditions;
            _settings = setings;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]FilterParemetersVolunteers filter)
        {
            var volunteers = _unitOfWork.Volunteers.GetAll().Where(c => !c.IsDeleted);
            volunteers = _filterConditions.GetVolunteers(volunteers, filter);

            if (volunteers == null)
            {
                return BadRequest();
            }

            var volunteersDTO = volunteers.Select(v =>
            new VolunteerDTO()
            {
                ID = v.ID,
                Name = v.FullName.Name,
                Surname = v.FullName.Surname,
                Patronymic = v.FullName.Patronymic,
                Birthday = v.Birthday,
                Rating = v.Rating,
                EmailID = v.EmailID,
                PhotoPath = _settings.Value.ServerURL + v.Avatar,
                AddressID = v.AddressID
            });

            return Ok(volunteersDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var volunteer = await _unitOfWork.Volunteers.GetById(id);

            if (volunteer == null)
            {
                return BadRequest();
            }

            var volunteerDTO = new VolunteerDTO()
            {
                ID = volunteer.ID,
                Name = volunteer.FullName.Name,
                Surname = volunteer.FullName.Surname,
                Patronymic = volunteer.FullName.Patronymic,
                Birthday = volunteer.Birthday,
                Rating = volunteer.Rating,
                EmailID = volunteer.EmailID,
                PhotoPath = _settings.Value.ServerURL + volunteer.Avatar,
                AddressID = volunteer.AddressID
            };

            return Ok(volunteerDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Volunteer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]VolunteerDTO volunteerDTO)
        {
            if (!_volunteerValidator.IsValid(volunteerDTO))
            {
                return BadRequest();
            }

            var pathPhoto = String.Empty;

            if (volunteerDTO.Avatar != null)
            {
                var fileName = volunteerDTO.Name + volunteerDTO.Surname
                        + volunteerDTO.Patronymic + DateTime.Now.Ticks;

                pathPhoto = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.Volunteer), volunteerDTO.Avatar);
            }

            var volunteer = new Volunteer()
            {
                Rating = volunteerDTO.Rating,
                Birthday = volunteerDTO.Birthday,
                FullName = new FullName()
                {
                    Name = volunteerDTO.Name,
                    Surname = volunteerDTO.Surname,
                    Patronymic = volunteerDTO.Patronymic
                },

                AddressID = volunteerDTO.AddressID,
                Avatar = pathPhoto,
                EmailID = volunteerDTO.EmailID,
            };

            await _unitOfWork.Volunteers.Create(volunteer);
            _unitOfWork.SaveChangesAsync();

            volunteerDTO.ID = volunteer.ID;
            volunteerDTO.PhotoPath = volunteer.Avatar;
            volunteerDTO.Avatar = null;

            return Created("api/v1/volunteers/" + volunteer.ID, volunteerDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Volunteer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromQuery]int id, [FromForm]VolunteerDTO volunteerDTO)
        {
            if (!_volunteerValidator.IsValid(volunteerDTO))
            {
                return BadRequest();
            }

            var volunteer = await _unitOfWork.Volunteers.GetById(volunteerDTO.ID);

            if (volunteer == null)
            {
                return BadRequest();
            }

            volunteer.FullName.Name = volunteerDTO.Name;
            volunteer.FullName.Patronymic = volunteerDTO.Patronymic;
            volunteer.FullName.Surname = volunteerDTO.Surname;
            volunteer.Birthday = volunteerDTO.Birthday;
            volunteer.Rating = volunteerDTO.Rating;
            volunteer.AddressID = volunteerDTO.AddressID;
            volunteer.EmailID = volunteerDTO.EmailID;

            if (volunteerDTO.Avatar != null)
            {
                var fileName = volunteerDTO.Name + volunteerDTO.Surname
                        + volunteerDTO.Patronymic + DateTime.Now.Ticks;

                volunteer.Avatar = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.Volunteer), volunteerDTO.Avatar);
            }

            _unitOfWork.Volunteers.Update(volunteer);
            _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var volunteer = await _unitOfWork.Volunteers.GetById(id);

            if (volunteer == null)
            {
                return BadRequest();
            }

            volunteer.IsDeleted = true;

            _unitOfWork.Volunteers.Update(volunteer);
            _unitOfWork.SaveChangesAsync();

            return Ok();
        }
    }
}