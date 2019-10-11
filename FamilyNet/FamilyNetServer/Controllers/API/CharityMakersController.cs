using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.DTO;
using FamilyNetServer.Models;
using FamilyNetServer.FileUploaders;
using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Validators;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CharityMakersController : ControllerBase
    {
        #region private
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ICharityMakersSelection _selection;
        private readonly ICharityMakerValidator _validator;
        private readonly IFileUploader _fileUploader;
        #endregion

        #region ctro
        public CharityMakersController(IUnitOfWorkAsync unitOfWork,
             ICharityMakersSelection selection, ICharityMakerValidator validator,
             IFileUploader fileUploader)
        {
            _unitOfWork = unitOfWork;
            _selection = selection;
            _validator = validator;
            _fileUploader = fileUploader;
        }
        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]string name, [FromQuery] float rating,
                                    [FromQuery]int rows, [FromQuery]int page)
        {
            var charityMakerContainer = _unitOfWork.CharityMakers.GetAll().Where(p => p.IsDeleted == false);
            charityMakerContainer = _selection.GetFiltered(charityMakerContainer, name, rating);

            if (rows != 0 && page != 0)
            {
                charityMakerContainer = charityMakerContainer.Skip(rows * page).Take(rows);
            }

            if (charityMakerContainer == null)
            {
                return BadRequest();
            }

            var charityMakerDTO = new List<CharityMakerDTO>();

            foreach (var charityMaker in charityMakerContainer)
            {
                charityMakerDTO.Add(new CharityMakerDTO
                {
                    PhotoPath = charityMaker.Avatar,
                    Birthday = charityMaker.Birthday,
                    EmailID = charityMaker.EmailID,
                    ID = charityMaker.ID,
                    Name = charityMaker.FullName.Name,
                    Patronymic = charityMaker.FullName.Patronymic,
                    Surname = charityMaker.FullName.Surname,
                    CharityMakerID = charityMaker.ID,
                    Rating = charityMaker.Rating
                });

            }

            return Ok(charityMakerDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var charityMaker = await _unitOfWork.CharityMakers.GetById(id);

            if (charityMaker == null)
            {
                return BadRequest();
            }

            var charityMakerDTO = new CharityMakerDTO()
            {
                Birthday = charityMaker.Birthday,
                ID = charityMaker.ID,
                Name = charityMaker.FullName.Name,
                CharityMakerID = charityMaker.ID,
                Patronymic = charityMaker.FullName.Patronymic,
                Rating = charityMaker.Rating,
                Surname = charityMaker.FullName.Surname,
                EmailID = charityMaker.EmailID,
                PhotoPath = charityMaker.Avatar
            };

            return Ok(charityMakerDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] CharityMakerDTO charityMakerDTO)
        {
            if (!_validator.IsValid(charityMakerDTO))
            {
                return BadRequest();
            }

            var pathPhoto = String.Empty;

            if (charityMakerDTO.Avatar != null)
            {
                var fileName = charityMakerDTO.Name + charityMakerDTO.Surname
                        + charityMakerDTO.Patronymic + DateTime.Now.Ticks;

                pathPhoto = _fileUploader.CopyFile(fileName,
                        nameof(DirectoryUploadName.CharityMaker), charityMakerDTO.Avatar);
            }

            var charityMaker = new CharityMaker()
            {
                Rating = charityMakerDTO.Rating,
                Birthday = charityMakerDTO.Birthday,
                FullName = new FullName()
                {
                    Name = charityMakerDTO.Name,
                    Surname = charityMakerDTO.Surname,
                    Patronymic = charityMakerDTO.Patronymic
                },

                ID = charityMakerDTO.CharityMakerID,
                Avatar = pathPhoto,
                EmailID = charityMakerDTO.EmailID,
            };

            await _unitOfWork.CharityMakers.Create(charityMaker);
            _unitOfWork.SaveChangesAsync();

            return Created("api/v1/charityMakers/" + charityMaker.ID, charityMaker);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromQuery]int id, [FromForm]CharityMakerDTO charityMakerDTO)
        {
            if (!_validator.IsValid(charityMakerDTO))
            {
                return BadRequest();
            }

            var charityMaker = await _unitOfWork.CharityMakers.GetById(charityMakerDTO.ID);

            if (charityMaker == null)
            {
                return BadRequest();
            }

            charityMaker.FullName.Name = charityMakerDTO.Name;
            charityMaker.FullName.Patronymic = charityMakerDTO.Patronymic;
            charityMaker.FullName.Surname = charityMakerDTO.Surname;
            charityMaker.Birthday = charityMakerDTO.Birthday;
            charityMaker.Rating = charityMakerDTO.Rating;
            charityMaker.ID = charityMakerDTO.CharityMakerID;
            charityMaker.EmailID = charityMakerDTO.EmailID;

            if (charityMakerDTO.Avatar != null)
            {
                var fileName = charityMakerDTO.Name + charityMakerDTO.Surname
                        + charityMakerDTO.Patronymic + DateTime.Now.Ticks;

                charityMaker.Avatar = _fileUploader.CopyFile(fileName,
                        nameof(DirectoryUploadName.Children), charityMakerDTO.Avatar);
            }

            _unitOfWork.CharityMakers.Update(charityMaker);
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

            var child = await _unitOfWork.CharityMakers.GetById(id);

            if (child == null)
            {
                return BadRequest();
            }

            child.IsDeleted = true;

            _unitOfWork.CharityMakers.Update(child);
            _unitOfWork.SaveChangesAsync();

            return Ok();
        }
    }
}