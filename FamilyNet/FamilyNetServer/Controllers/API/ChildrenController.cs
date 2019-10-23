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
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API
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

        #endregion

        #region ctor

        public ChildrenController(IFileUploader fileUploader,
                                  IUnitOfWork unitOfWork,
                                  IChildValidator childValidator,
                                  IFilterConditionsChildren filterConditions,
                                  IOptionsSnapshot<ServerURLSettings> setings)
        {
            _fileUploader = fileUploader;
            _unitOfWork = unitOfWork;
            _childValidator = childValidator;
            _filterConditions = filterConditions;
            _settings = setings;
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
                Rating = c.Rating
            });

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
                return BadRequest();
            }

            var childDTO = new ChildDTO()
            {
                Birthday = child.Birthday,
                ID = child.ID,
                Name = child.FullName.Name,
                ChildrenHouseID = child.OrphanageID ?? 0,
                Patronymic = child.FullName.Patronymic,
                Rating = child.Rating,
                Surname = child.FullName.Surname,
                EmailID = child.EmailID,
                PhotoPath = _settings.Value.ServerURL + child.Avatar
            };

            return Ok(childDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Orphan")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]ChildDTO childDTO)
        {
            if (!_childValidator.IsValid(childDTO))
            {
                return BadRequest();
            }

            var pathPhoto = String.Empty;

            if (childDTO.Avatar != null)
            {
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
                return BadRequest();
            }

            var child = await _unitOfWork.Orphans.GetById(childDTO.ID);

            if (child == null)
            {
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
                var fileName = childDTO.Name + childDTO.Surname
                        + childDTO.Patronymic + DateTime.Now.Ticks;

                child.Avatar = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.Children), childDTO.Avatar);
            }

            _unitOfWork.Orphans.Update(child);
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

            var child = await _unitOfWork.Orphans.GetById(id);

            if (child == null)
            {
                return BadRequest();
            }

            child.IsDeleted = true;

            _unitOfWork.Orphans.Update(child);
            _unitOfWork.SaveChangesAsync();

            return Ok();
        }
    }
}