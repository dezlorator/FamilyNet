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
    public class ChildrenController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IFileUploader _fileUploader;
        private readonly IChildValidator _childValidator;
        private readonly IFilterConditionsChildren _filterConditions;

        #endregion

        #region ctor

        public ChildrenController(IFileUploader fileUploader,
                                  IUnitOfWorkAsync unitOfWork,
                                  IChildValidator childValidator,
                                  IFilterConditionsChildren filterConditions)
        {
            _fileUploader = fileUploader;
            _unitOfWork = unitOfWork;
            _childValidator = childValidator;
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
            var children = _unitOfWork.Orphans.GetAll().Where(c => !c.IsDeleted);
            children = _filterConditions.GetOrphans(children, name, rating, age);

            if (rows != 0 && page != 0)
            {
                children = children.Skip(rows * page).Take(rows);
            }

            if (children == null)
            {
                return BadRequest();
            }

            var childrenDTO = new List<ChildDTO>();

            foreach (var c in children)
            {
                var childDTO = new ChildDTO()
                {
                    PhotoPath = c.Avatar,
                    Birthday = c.Birthday,
                    EmailID = c.EmailID,
                    ID = c.ID,
                    Name = c.FullName.Name,
                    Patronymic = c.FullName.Patronymic,
                    Surname = c.FullName.Surname,
                    OrphanageID = c.OrphanageID ?? 0,
                    Rating = c.Rating
                };

                childrenDTO.Add(childDTO);
            }

            return Ok(childrenDTO);
        }

        [HttpGet("{id}")]
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
                OrphanageID = child.OrphanageID ?? 0,
                Patronymic = child.FullName.Patronymic,
                Rating = child.Rating,
                Surname = child.FullName.Surname,
                EmailID = child.EmailID,
                PhotoPath = child.Avatar
            };

            return Ok(childDTO);
        }

        [HttpPost]
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

                pathPhoto = _fileUploader.CopyFile(fileName,
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

                OrphanageID = childDTO.OrphanageID,
                Avatar = pathPhoto,
                EmailID = childDTO.EmailID,
            };

            await _unitOfWork.Orphans.Create(child);
            _unitOfWork.SaveChangesAsync();

            return Created("api/v1/children/" + child.ID, child);
        }

        [HttpPut("{id}")]
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
            child.OrphanageID = childDTO.OrphanageID;
            child.EmailID = childDTO.EmailID;

            if (childDTO.Avatar != null)
            {
                var fileName = childDTO.Name + childDTO.Surname
                        + childDTO.Patronymic + DateTime.Now.Ticks;

                child.Avatar = _fileUploader.CopyFile(fileName,
                        nameof(DirectoryUploadName.Children), childDTO.Avatar);
            }

            _unitOfWork.Orphans.Update(child);
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