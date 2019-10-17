using FamilyNetServer.DTO;
using FamilyNetServer.Enums;
using FamilyNetServer.FileUploaders;
using FamilyNetServer.Filters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VolunteersController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IFileUploader _fileUploader;
        private readonly IVolunteerValidator _volunteerValidator;
        private readonly IFilterConditionsVolunteers _filterConditions;

        #endregion

        #region ctor

        public VolunteersController(IFileUploader fileUploader,
                                  IUnitOfWorkAsync unitOfWork,
                                  IVolunteerValidator volunteerValidator,
                                  IFilterConditionsVolunteers filterConditions)
        {
            _fileUploader = fileUploader;
            _unitOfWork = unitOfWork;
            _volunteerValidator = volunteerValidator;
            _filterConditions = filterConditions;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]string name,
                            [FromQuery]float rating,
                            [FromQuery]int age,
                            [FromQuery]int rows,
                            [FromQuery]int page)
        {
            var volunteer = _unitOfWork.Volunteers.GetAll().Where(c => !c.IsDeleted);
            volunteer = _filterConditions.GetVolunteers(volunteer, name, rating, age);

            if (rows != 0 && page != 0)
            {
                volunteer = volunteer.Skip(rows * page).Take(rows);
            }

            if (volunteer == null)
            {
                return BadRequest();
            }

            var volunteersDTO = new List<VolunteerDTO>();

            foreach (var c in volunteer)
            {
                var volunteerDTO = new VolunteerDTO()
                {
                    PhotoPath = c.Avatar,
                    Birthday = c.Birthday,
                    EmailID = c.EmailID,
                    ID = c.ID,
                    Name = c.FullName.Name,
                    Patronymic = c.FullName.Patronymic,
                    Surname = c.FullName.Surname,
                    Rating = c.Rating,
                    AddressID = c.AddressID
                };

                volunteersDTO.Add(volunteerDTO);
            }

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
                Birthday = volunteer.Birthday,
                ID = volunteer.ID,
                Name = volunteer.FullName.Name,
                Patronymic = volunteer.FullName.Patronymic,
                Rating = volunteer.Rating,
                Surname = volunteer.FullName.Surname,
                EmailID = volunteer.EmailID,
                PhotoPath = volunteer.Avatar,
                AddressID = volunteer.AddressID,
            };

            return Ok(volunteerDTO);
        }

        [HttpPost]
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

                pathPhoto = _fileUploader.CopyFile(fileName,
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

            return Created("api/v1/volunteers/" + volunteer.ID, volunteer);
        }

        [HttpPut("{id}")]
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

                volunteer.Avatar = _fileUploader.CopyFile(fileName,
                        nameof(DirectoryUploadName.Volunteer), volunteerDTO.Avatar);
            }

            _unitOfWork.Volunteers.Update(volunteer);
            _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
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