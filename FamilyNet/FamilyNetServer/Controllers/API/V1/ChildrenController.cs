using DataTransferObjects;
using FamilyNetServer.Configuration;
using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FamilyNetServer.HttpHandlers;
using Microsoft.EntityFrameworkCore;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ChildrenController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploader _fileUploader;
        private readonly IChildValidator _childValidator;
        private readonly IFilterConditionsChildren _filterConditions;
        private readonly IOptionsSnapshot<ServerURLSettings> _settings;
        private readonly ILogger<ChildrenController> _logger;
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public ChildrenController(IFileUploader fileUploader,
                                  IUnitOfWork unitOfWork,
                                  IChildValidator childValidator,
                                  IFilterConditionsChildren filterConditions,
                                  IOptionsSnapshot<ServerURLSettings> setings,
                                  ILogger<ChildrenController> logger,
                                  IIdentityExtractor identityExtractor)
        {
            _fileUploader = fileUploader;
            _unitOfWork = unitOfWork;
            _childValidator = childValidator;
            _filterConditions = filterConditions;
            _settings = setings;
            _logger = logger;
            _identityExtractor = identityExtractor;
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery]FilterParemetersChildren filter)
        {
            _logger.LogInformation("{info}",
                "Endpoint Children/api/v1 GetAll was called");

            var children = _unitOfWork.Orphans.GetAll().Where(c => !c.IsDeleted);
            children = _filterConditions.GetOrphans(children, filter);

            if (children == null)
            {
                _logger.LogInformation("{status}{info}",
                    StatusCodes.Status400BadRequest,
                    "List of Children is empty");

                return BadRequest();
            }

            var childrenDTO = await children.Select(c =>
                new ChildDTO()
                {
                    PhotoPath = _settings.Value.ServerURL + c.Avatar,
                    Birthday = c.Birthday,
                    EmailID = c.EmailID,
                    ID = c.ID,
                    Name = c.FullName.Name,
                    Patronymic = c.FullName.Patronymic,
                    Surname = c.FullName.Surname,
                    ChildrenHouseID = c.OrphanageID ?? 0,
                    ChildrenHouseName = c.Orphanage.Name,
                    Rating = c.Rating
                }).ToListAsync();

            _logger.LogInformation("{status}, {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(childrenDTO));

            return Ok(childrenDTO);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                $"Endpoint Children/api/v1 GetById({id}) was called");

            var child = await _unitOfWork.Orphans.GetById(id);

            if (child == null)
            {
                _logger.LogError("{info}{status}",
                    $"Child wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var childDTO = new ChildDTO()
            {
                Birthday = child.Birthday,
                ID = child.ID,
                Name = child.FullName?.Name,
                ChildrenHouseID = child.OrphanageID ?? 0,
                ChildrenHouseName = child.Orphanage?.Name,
                Patronymic = child.FullName?.Patronymic,
                Rating = child.Rating,
                Surname = child.FullName.Surname,
                EmailID = child.EmailID,
                PhotoPath = _settings.Value.ServerURL + child.Avatar
            };

            _logger.LogInformation("{status}{json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(childDTO));

            return Ok(childDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Orphan")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]ChildDTO childDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Children/api/v1 [POST] was called", userId, token);

            if (!_childValidator.IsValid(childDTO))
            {
                _logger.LogWarning("{status}{token}{userId}",
                    StatusCodes.Status400BadRequest, token, userId);

                return BadRequest();
            }

            var pathPhoto = String.Empty;

            if (childDTO.Avatar != null)
            {
                _logger.LogInformation("{info}", "ChildDTO has file photo.");

                var fileName = childDTO.Name + childDTO.Surname
                    + childDTO.Patronymic + DateTime.Now.Ticks;

                pathPhoto = _fileUploader.CopyFileToServer(fileName,
                    nameof(DirectoryUploadName.Children), childDTO.Avatar);
            }

            var child = new Orphan()
            {
                Rating = childDTO.Rating,
                Birthday = childDTO.Birthday,
                FullName = new FullName()
                {
                    Name = childDTO.Name,
                    Surname = childDTO.Surname,
                    Patronymic = childDTO.Patronymic
                },

                OrphanageID = childDTO.ChildrenHouseID,
                Avatar = pathPhoto,
                EmailID = childDTO.EmailID,
            };

            await _unitOfWork.Orphans.Create(child);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Child was saved [id:{child.ID}]");

            return Created(child.ID.ToString(), new ChildDTO());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Orphan")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromQuery]int id, [FromForm]ChildDTO childDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Children/api/v1 [PUT] was called", userId, token);

            if (!_childValidator.IsValid(childDTO))
            {
                _logger.LogError("{userId} {token} {status} {info}", userId,
                    token, StatusCodes.Status400BadRequest.ToString(),
                    "ChildDTO is invalid");

                return BadRequest();
            }

            var child = await _unitOfWork.Orphans.GetById(childDTO.ID);

            if (child == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Child was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            child.FullName.Name = childDTO.Name;
            child.FullName.Patronymic = childDTO.Patronymic;
            child.FullName.Surname = childDTO.Surname;
            child.Birthday = childDTO.Birthday;
            child.Rating = childDTO.Rating;
            child.OrphanageID = childDTO.ChildrenHouseID;
            child.EmailID = childDTO.EmailID;

            if (childDTO.Avatar != null)
            {
                _logger.LogInformation("{info}", "ChildDTO has file photo.");

                var fileName = childDTO.Name + childDTO.Surname
                        + childDTO.Patronymic + DateTime.Now.Ticks;

                child.Avatar = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.Children), childDTO.Avatar);
            }

            _unitOfWork.Orphans.Update(child);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{token}{userId}{status}{info}",
                 token, userId, StatusCodes.Status204NoContent,
                 $"Child was updated [id:{child.ID}]");

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
                "Endpoint Children/api/v1 [DELETE] was called", userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            var child = await _unitOfWork.Orphans.GetById(id);

            if (child == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Child was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            child.IsDeleted = true;

            _unitOfWork.Orphans.Update(child);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"Child.IsDelete was updated [id:{id}]", userId, token);

            return Ok();
        }
    }
}