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

        #endregion

        #region ctor

        public ChildrenController(IFileUploader fileUploader,
                                  IUnitOfWork unitOfWork,
                                  IChildValidator childValidator,
                                  IFilterConditionsChildren filterConditions,
                                  IOptionsSnapshot<ServerURLSettings> setings,
                                  ILogger<ChildrenController> logger)
        {
            _fileUploader = fileUploader;
            _unitOfWork = unitOfWork;
            _childValidator = childValidator;
            _filterConditions = filterConditions;
            _settings = setings;
            _logger = logger;
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]FilterParemetersChildren filter)
        {
            var children = _unitOfWork.Orphans.GetAll().Where(c => !c.IsDeleted);
            children = _filterConditions.GetOrphans(children, filter);

            if (children == null)
            {
                _logger.LogInformation("Bad request[400]. Child wasn't found.");
                return BadRequest();
            }

            var childrenDTO = children.Select(c =>
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
            });

            _logger.LogInformation("Retrn Ok[200]. List of children was sent");
            return Ok(childrenDTO);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var child = await _unitOfWork.Orphans.GetById(id);

            if (child == null)
            {
                _logger.LogError("Bad request[400]. Child wasn't found");
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

            _logger.LogInformation("Return Ok[200]. Child was sent.");
            return Ok(childDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Orphan")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]ChildDTO childDTO)
        {
            var userId = User.Identity.Name;
            _logger.LogInformation("Endpoint was called by User {userId}", userId);

            if (!_childValidator.IsValid(childDTO))
            {
                _logger.LogWarning("Bad request[400]. ChildDTO is not valid.");
                return BadRequest();
            }

            var pathPhoto = String.Empty;

            if (childDTO.Avatar != null)
            {
                _logger.LogInformation("ChildDTO has file photo.");
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
            _logger.LogInformation("Return Created[201].New child was added.");

            return Created("api/v1/children/" + child.ID, new ChildDTO());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Orphan")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromQuery]int id, [FromForm]ChildDTO childDTO)
        {
            if (!_childValidator.IsValid(childDTO))
            {
                _logger.LogError("Bad request[400]. ChildDTO is not valid");
                return BadRequest();
            }

            var child = await _unitOfWork.Orphans.GetById(childDTO.ID);

            if (child == null)
            {
                _logger.LogError("Bad request. Child was not found by id");
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
                _logger.LogInformation("ChildDTO has file photo.");
                var fileName = childDTO.Name + childDTO.Surname
                        + childDTO.Patronymic + DateTime.Now.Ticks;

                child.Avatar = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.Children), childDTO.Avatar);
            }

            _unitOfWork.Orphans.Update(child);
            _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Return NoContent[204]. Child was updated.");

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
                _logger.LogError("Bad request[400]. Argument id is not valid");
                return BadRequest();
            }

            var child = await _unitOfWork.Orphans.GetById(id);

            if (child == null)
            {
                _logger.LogError("Bad request[400]. Chils wasn't found.");
                return BadRequest();
            }

            child.IsDeleted = true;

            _unitOfWork.Orphans.Update(child);
            _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Return Ok[200]. Child property IsDelete was updated.");

            return Ok();
        }
    }
}