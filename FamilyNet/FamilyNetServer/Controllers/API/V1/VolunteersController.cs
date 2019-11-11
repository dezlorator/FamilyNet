using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;
using Microsoft.Extensions.Options;
using FamilyNetServer.Configuration;
using FamilyNetServer.Filters.FilterParameters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using FamilyNetServer.HttpHandlers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FamilyNetServer.Controllers.API.V1
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
        private readonly ILogger<VolunteersController> _logger;
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public VolunteersController(IFileUploader fileUploader,
                                  IUnitOfWork unitOfWork,
                                  IVolunteerValidator volunteerValidator,
                                  IFilterConditionsVolunteers filterConditions,
                                  IOptionsSnapshot<ServerURLSettings> setings,
                                  ILogger<VolunteersController> logger,
                                  IIdentityExtractor identityExtractor)
        {
            _fileUploader = fileUploader;
            _unitOfWork = unitOfWork;
            _volunteerValidator = volunteerValidator;
            _filterConditions = filterConditions;
            _settings = setings;
            _logger = logger;
            _identityExtractor = identityExtractor;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery]FilterParemetersVolunteers filter)
        {
            _logger.LogInformation("{info}",
                "Endpoint Volunteers/api/v1 GetAll was called");

            var volunteers = _unitOfWork.Volunteers.GetAll().Where(c => !c.IsDeleted);
            volunteers = _filterConditions.GetVolunteers(volunteers, filter);

            if (volunteers == null)
            {
                _logger.LogInformation("{status}{info}",
                    StatusCodes.Status400BadRequest,
                    "List of Volunteers is empty");

                return BadRequest();
            }

            var volunteersDTO = await volunteers.Select(v =>
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
                }).ToListAsync();

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(volunteersDTO));

            return Ok(volunteersDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                $"Endpoint Volunteers/api/v1 GetById({id}) was called");

            var volunteer = await _unitOfWork.Volunteers.GetById(id);

            if (volunteer == null)
            {
                _logger.LogError("{info}{status}",
                    $"Voluntees wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

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

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(volunteerDTO));

            return Ok(volunteerDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Volunteer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]VolunteerDTO volunteerDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Volunteers/api/v1 [POST] was called", userId, token);

            if (!_volunteerValidator.IsValid(volunteerDTO))
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                    StatusCodes.Status400BadRequest, token, userId,
                    "VolunteerDTO is invalid");

                return BadRequest();
            }

            var pathPhoto = String.Empty;

            if (volunteerDTO.Avatar != null)
            {
                _logger.LogInformation("{info}", "VolunteerDTO has file photo.");

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

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Volunteer was saved [id:{volunteer.ID}]");

            return Created("api/v1/volunteers/" + volunteer.ID, volunteerDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Volunteer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromQuery]int id, [FromForm]VolunteerDTO volunteerDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Volunteers/api/v1 [PUT] was called", userId, token);

            if (!_volunteerValidator.IsValid(volunteerDTO))
            {
                _logger.LogError("{userId} {token} {status} {info}",
                    userId, token, StatusCodes.Status400BadRequest,
                    "VolunteerDTO is invalid");

                return BadRequest();
            }

            var volunteer = await _unitOfWork.Volunteers.GetById(volunteerDTO.ID);

            if (volunteer == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Volunteer was not found [id:{id}]", userId, token);

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
                _logger.LogInformation("{info}", "VolunteerDTO has file photo.");

                var fileName = volunteerDTO.Name + volunteerDTO.Surname
                    + volunteerDTO.Patronymic + DateTime.Now.Ticks;

                volunteer.Avatar = _fileUploader.CopyFileToServer(fileName,
                    nameof(DirectoryUploadName.Volunteer), volunteerDTO.Avatar);
            }

            _unitOfWork.Volunteers.Update(volunteer);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{token}{userId}{status}{info}",
                 token, userId, StatusCodes.Status204NoContent,
                 $"Volunteer was updated [id:{volunteer.ID}]");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Volunteers/api/v1 [DELETE] was called",
                userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            var volunteer = await _unitOfWork.Volunteers.GetById(id);

            if (volunteer == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Volunteer was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            volunteer.IsDeleted = true;

            _unitOfWork.Volunteers.Update(volunteer);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"Volunteer.IsDelete was updated [id:{id}]", userId, token);

            return Ok();
        }
    }
}