using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.Models;
using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Validators;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Configuration;
using Microsoft.Extensions.Options;
using DataTransferObjects;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CharityMakersController : ControllerBase
    {
        #region private
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICharityMakersSelection _selection;
        private readonly ICharityMakerValidator _validator;
        private readonly IFileUploader _fileUploader;
        private readonly IOptionsSnapshot<ServerURLSettings> _settings;
        private readonly ILogger<CharityMakersController> _logger;

        #endregion

        #region ctro
        public CharityMakersController(IUnitOfWork unitOfWork,
             ICharityMakersSelection selection, ICharityMakerValidator validator,
             IFileUploader fileUploader,
             IOptionsSnapshot<ServerURLSettings> settings,
             ILogger<CharityMakersController> logger)
        {
            _unitOfWork = unitOfWork;
            _selection = selection;
            _validator = validator;
            _fileUploader = fileUploader;
            _settings = settings;
            _logger = logger;
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

            if (rows > 0 && page > 0)
            {
                _logger.LogInformation("Paging were used");
                charityMakerContainer = charityMakerContainer.Skip(rows * page).Take(rows);
            }

            if (charityMakerContainer == null)
            {
                _logger.LogError("Bad request. No charity maker found");
                return BadRequest();
            }

            var charityMakerDTO = new List<CharityMakerDTO>();

            foreach (var charityMaker in charityMakerContainer)
            {
                charityMakerDTO.Add(new CharityMakerDTO
                {
                    PhotoPath = _settings.Value.ServerURL + charityMaker.Avatar,
                    Birthday = charityMaker.Birthday,
                    EmailID = charityMaker.EmailID,
                    ID = charityMaker.ID,
                    Name = charityMaker.FullName.Name,
                    Patronymic = charityMaker.FullName.Patronymic,
                    Surname = charityMaker.FullName.Surname,
                    Rating = charityMaker.Rating,
                    AdressID = charityMaker.AddressID?? 0
                });

            }

            _logger.LogInformation("List of charity makers was sent");
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
                _logger.LogError("Bad request. No charity maker found");
                return BadRequest();
            }

            var charityMakerDTO = new CharityMakerDTO()
            {
                Birthday = charityMaker.Birthday,
                ID = charityMaker.ID,
                Name = charityMaker.FullName.Name,
                AdressID = charityMaker.AddressID ?? 0,
                Patronymic = charityMaker.FullName.Patronymic,
                Rating = charityMaker.Rating,
                Surname = charityMaker.FullName.Surname,
                EmailID = charityMaker.EmailID,
                PhotoPath = _settings.Value.ServerURL + charityMaker.Avatar,
            };

            _logger.LogInformation("Charity maker was sent");
            return Ok(charityMakerDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] CharityMakerDTO charityMakerDTO)
        {
            if (!_validator.IsValid(charityMakerDTO))
            {
                _logger.LogError("Unfilled name, surname, patronymic, birthday or wrong id");
                return BadRequest();
            }

            var pathPhoto = String.Empty;

            if (charityMakerDTO.Avatar != null)
            {
                var fileName = charityMakerDTO.Name + charityMakerDTO.Surname
                        + charityMakerDTO.Patronymic + DateTime.Now.Ticks;

                pathPhoto = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.CharityMaker), charityMakerDTO.Avatar);
                _logger.LogInformation(string.Format("{0} - this path to photo was created", pathPhoto));
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

                ID = charityMakerDTO.ID,
                Avatar = pathPhoto,
                EmailID = charityMakerDTO.EmailID,
                AddressID = charityMakerDTO.AdressID
            };

            await _unitOfWork.CharityMakers.Create(charityMaker);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Charity maker was created");
            return Created("api/v1/charityMakers/" + charityMaker.ID, charityMaker);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromQuery]int id, [FromForm]CharityMakerDTO charityMakerDTO)
        {
            if (!_validator.IsValid(charityMakerDTO))
            {
                _logger.LogError("Unfilled name, surname, patronymic, birthday or wrong id");
                return BadRequest();
            }

            var charityMaker = await _unitOfWork.CharityMakers.GetById(charityMakerDTO.ID);

            if (charityMaker == null)
            {
                _logger.LogError("Bad request. No charity maker found");
                return BadRequest();
            }

            charityMaker.FullName.Name = charityMakerDTO.Name;
            charityMaker.FullName.Patronymic = charityMakerDTO.Patronymic;
            charityMaker.FullName.Surname = charityMakerDTO.Surname;
            charityMaker.Birthday = charityMakerDTO.Birthday;
            charityMaker.Rating = charityMakerDTO.Rating;
            charityMaker.ID = charityMakerDTO.ID;
            charityMaker.EmailID = charityMakerDTO.EmailID;

            if (charityMakerDTO.Avatar != null)
            {
                var fileName = charityMakerDTO.Name + charityMakerDTO.Surname
                        + charityMakerDTO.Patronymic + DateTime.Now.Ticks;

                charityMaker.Avatar = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.CharityMaker), charityMakerDTO.Avatar);
                _logger.LogInformation(string.Format("{0} - this path to photo was created",
                    charityMaker.Avatar));
            }

            _unitOfWork.CharityMakers.Update(charityMaker);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Charity maker was successfully updated");
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
                _logger.LogError("Wrong id - {0}", id);
                return BadRequest();
            }

            var child = await _unitOfWork.CharityMakers.GetById(id);

            if (child == null)
            {
                _logger.LogError("Bad request. No charity maker found");
                return BadRequest();
            }

            child.IsDeleted = true;

            _unitOfWork.CharityMakers.Update(child);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Charity maker was deleted");
            return Ok();
        }
    }
}