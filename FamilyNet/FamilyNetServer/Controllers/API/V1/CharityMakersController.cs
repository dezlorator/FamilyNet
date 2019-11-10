using System;
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
using FamilyNetServer.HttpHandlers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

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
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public CharityMakersController(IUnitOfWork unitOfWork,
             ICharityMakersSelection selection,
             ICharityMakerValidator validator,
             IFileUploader fileUploader,
             IOptionsSnapshot<ServerURLSettings> settings,
             ILogger<CharityMakersController> logger,
             IIdentityExtractor identityExtractor)
        {
            _unitOfWork = unitOfWork;
            _selection = selection;
            _validator = validator;
            _fileUploader = fileUploader;
            _settings = settings;
            _logger = logger;
            _identityExtractor = identityExtractor;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery]string name, [FromQuery] float rating,
                                    [FromQuery]int rows, [FromQuery]int page)
        {
            _logger.LogInformation("{info}",
                  "Endpoint CharityMakers/api/v1 GetAll was called");

            var charityMakerContainer = _unitOfWork.CharityMakers.GetAll().Where(p => p.IsDeleted == false);
            charityMakerContainer = _selection.GetFiltered(charityMakerContainer, name, rating);

            if (rows > 0 && page > 0)
            {
                _logger.LogInformation("{info}", "Paging were used");
                charityMakerContainer = charityMakerContainer.Skip(rows * page).Take(rows);
            }

            if (charityMakerContainer == null)
            {
                _logger.LogError("{info}{status}", "Charity maker was not found",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var charityMakerDTO = await charityMakerContainer.Select(charityMaker =>
            new CharityMakerDTO
            {
                PhotoPath = _settings.Value.ServerURL + charityMaker.Avatar,
                Birthday = charityMaker.Birthday,
                EmailID = charityMaker.EmailID,
                ID = charityMaker.ID,
                Name = charityMaker.FullName.Name,
                Patronymic = charityMaker.FullName.Patronymic,
                Surname = charityMaker.FullName.Surname,
                Rating = charityMaker.Rating,
                AdressID = charityMaker.AddressID ?? 0
            }).ToListAsync();

            _logger.LogInformation("{status}, {json}",
                           StatusCodes.Status200OK,
                           JsonConvert.SerializeObject(charityMakerDTO));

            return Ok(charityMakerDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                 $"Endpoint CharityMakers/api/v1 GetById({id}) was called");

            var charityMaker = await _unitOfWork.CharityMakers.GetById(id);

            if (charityMaker == null)
            {
                _logger.LogError("{info}{status}", $"CharityMaker wasn't found [id:{id}]",
                                   StatusCodes.Status400BadRequest);

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

            _logger.LogInformation("{status},{json}",
                           StatusCodes.Status200OK,
                           JsonConvert.SerializeObject(charityMakerDTO));

            return Ok(charityMakerDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] CharityMakerDTO charityMakerDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            if (!_validator.IsValid(charityMakerDTO))
            {
                _logger.LogError("{status}{token}{userId}{info}",
                   StatusCodes.Status400BadRequest,
                   token, userId,
                   "Unfilled name, surname, patronymic, birthday or wrong id");

                return BadRequest();
            }

            var pathPhoto = String.Empty;

            if (charityMakerDTO.Avatar != null)
            {
                _logger.LogInformation("{info}", "charityMakerDTO has file photo.");

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

            _logger.LogInformation("{token}{userId}{status}{info}",
                           token, userId, StatusCodes.Status201Created,
                           $"CharityMaker was saved [id:{charityMaker.ID}]");

            return Created("api/v1/charityMakers/" + charityMaker.ID, charityMaker);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, CharityMaker")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromQuery]int id, [FromForm]CharityMakerDTO charityMakerDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint CharityMakers/api/v1 [PUT] was called", userId, token);

            if (!_validator.IsValid(charityMakerDTO))
            {
                _logger.LogError("{userId} {token} {status} {info}", userId, token,
                    StatusCodes.Status400BadRequest.ToString(),
                    "Unfilled name, surname, patronymic, birthday or wrong id");

                return BadRequest();
            }

            var charityMaker = await _unitOfWork.CharityMakers.GetById(charityMakerDTO.ID);

            if (charityMaker == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                                   StatusCodes.Status400BadRequest,
                                   $"CharityMaker was not found [id:{id}]",
                                   userId, token);

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
                _logger.LogInformation("{info}", "CharityMakerDTO has file photo.");

                var fileName = charityMakerDTO.Name + charityMakerDTO.Surname
                        + charityMakerDTO.Patronymic + DateTime.Now.Ticks;

                charityMaker.Avatar = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.CharityMaker), charityMakerDTO.Avatar);
                _logger.LogInformation(string.Format("{0} - this path to photo was created",
                    charityMaker.Avatar));
            }

            _unitOfWork.CharityMakers.Update(charityMaker);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{token}{userId}{status}{info}",
                             token, userId, StatusCodes.Status204NoContent,
                             $"CharityMaker was updated [id:{charityMaker.ID}]");

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
              "Endpoint CharityMakers/api/v1 [DELETE] was called", userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                   StatusCodes.Status400BadRequest,
                   $"Argument id is not valid [id:{id}]",
                   userId, token);

                return BadRequest();
            }

            var child = await _unitOfWork.CharityMakers.GetById(id);

            if (child == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                StatusCodes.Status400BadRequest,
                $"Charity maker was not found [id:{id}]",
                userId, token);

                return BadRequest();
            }

            child.IsDeleted = true;

            _unitOfWork.CharityMakers.Update(child);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{status} {info} {userId} {token}",
               StatusCodes.Status200OK,
               $"CharityMaker.IsDelete was updated [id:{id}]",
               userId, token);
            return Ok();
        }
    }
}