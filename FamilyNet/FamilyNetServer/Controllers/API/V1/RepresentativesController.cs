using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using FamilyNetServer.Uploaders;
using FamilyNetServer.HttpHandlers;
using FamilyNetServer.Configuration;
using DataTransferObjects;
using Newtonsoft.Json;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RepresentativesController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploader _fileUploader;
        private readonly IRepresentativeValidator _representativeValidator;
        private readonly IFilterConditionsRepresentatives _filterConditions;
        private readonly IOptionsSnapshot<ServerURLSettings> _settings;
        private readonly ILogger<RepresentativesController> _logger;
        private readonly IIdentityExtractor _identityExtractor;

        private string _url
        {
            get
            {
                return string.Format($"{Request.Scheme}://{Request.Host.Value}/");
            }
        }

        #endregion

        #region ctor

        public RepresentativesController(IFileUploader fileUploader,
                                  IUnitOfWork unitOfWork,
                                  IRepresentativeValidator representativeValidator,
                                  IFilterConditionsRepresentatives filterConditions,
                                  IOptionsSnapshot<ServerURLSettings> settings,
                                  IIdentityExtractor identityExtractor,
                                  ILogger<RepresentativesController> logger)
        {
            _fileUploader = fileUploader;
            _unitOfWork = unitOfWork;
            _representativeValidator = representativeValidator;
            _filterConditions = filterConditions;
            _settings = settings;
            _identityExtractor = identityExtractor;
            _logger = logger;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllAsync([FromQuery] FilterParametersRepresentatives filter)
        {
            _logger.LogInformation("{info}",
                "Endpoint Representatives/api/v1 GetAll was called");

            var representatives = _unitOfWork.Representatives.GetAll()
                .Where(r => !r.IsDeleted);
            representatives = _filterConditions
                .GetRepresentatives(representatives, filter);

            if (representatives == null)
            {
                _logger.LogInformation("{status}{info}",
                    StatusCodes.Status400BadRequest,
                    "List of Representatives is empty");

                return BadRequest();
            }

            var representativesDTO = await representatives.Select(r =>
                new RepresentativeDTO()
                {
                    PhotoPath = _settings.Value.ServerURL + r.Avatar,
                    Birthday = r.Birthday,
                    EmailID = r.EmailID,
                    ID = r.ID,
                    Name = r.FullName.Name,
                    Patronymic = r.FullName.Patronymic,
                    Surname = r.FullName.Surname,
                    ChildrenHouseID = r.OrphanageID,
                    Rating = r.Rating
                }).ToListAsync();

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(representativesDTO));

            return Ok(representativesDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                $"Endpoint Representatives/api/v1 GetById({id}) was called");

            var represenntative = await _unitOfWork.Representatives.GetById(id);

            if (represenntative == null)
            {
                _logger.LogError("{info}{status}",
                    $"Representative wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var representativeDTO = new RepresentativeDTO()
            {
                Birthday = represenntative.Birthday,
                ID = represenntative.ID,
                Name = represenntative.FullName.Name,
                ChildrenHouseID = represenntative.OrphanageID,
                Patronymic = represenntative.FullName.Patronymic,
                Rating = represenntative.Rating,
                Surname = represenntative.FullName.Surname,
                EmailID = represenntative.EmailID,
                PhotoPath = _settings.Value.ServerURL + represenntative.Avatar
            };

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(representativeDTO));

            return Ok(representativeDTO);
        }

        [HttpGet("byChildrenHouse/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByChildrenHouseId(int id)
        {
            var childrenHouse =
                await _unitOfWork.Orphanages.GetById(id);

            if (childrenHouse == null)
            {
                return BadRequest();
            }

            var representatives = childrenHouse.Representatives;

            if (representatives == null)
            {
                return BadRequest();
            }

            var representativesDTO = representatives.Select(r =>
             new RepresentativeDTO()
             {
                 PhotoPath = _settings.Value.ServerURL + r.Avatar,
                 Birthday = r.Birthday,
                 EmailID = r.EmailID,
                 ID = r.ID,
                 Name = r.FullName.Name,
                 Patronymic = r.FullName.Patronymic,
                 Surname = r.FullName.Surname,
                 ChildrenHouseID = r.OrphanageID,
                 Rating = r.Rating
             });

            return Ok(representativesDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]RepresentativeDTO representativeDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Representatives/api/v1 [POST] was called", userId, token);

            if (!_representativeValidator.IsValid(representativeDTO))
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                    StatusCodes.Status400BadRequest, token, userId,
                    "RepresentativeDTO is invalid");

                return BadRequest();
            }

            var pathPhoto = String.Empty;

            if (representativeDTO.Avatar != null)
            {
                _logger.LogInformation("{info}", "RepresentativeDTO has file photo.");

                var fileName = representativeDTO.Name + representativeDTO.Surname
                    + representativeDTO.Patronymic + DateTime.Now.Ticks;

                pathPhoto = _fileUploader.CopyFileToServer(fileName,
                    nameof(DirectoryUploadName.Representatives),
                    representativeDTO.Avatar);
            }

            var representative = new Representative()
            {
                Rating = representativeDTO.Rating,
                Birthday = representativeDTO.Birthday,
                FullName = new FullName()
                {
                    Name = representativeDTO.Name,
                    Surname = representativeDTO.Surname,
                    Patronymic = representativeDTO.Patronymic
                },

                OrphanageID = representativeDTO.ChildrenHouseID,
                Avatar = pathPhoto,
                EmailID = representativeDTO.EmailID,
            };

            await _unitOfWork.Representatives.Create(representative);
            _unitOfWork.SaveChanges();

            representativeDTO.ID = representative.ID;
            representativeDTO.PhotoPath = representative.Avatar;

            var id = User.Identity.Name;
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            user.PersonID = representative.ID;
            await _unitOfWork.UserManager.UpdateAsync(user);

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Representative was saved [id:{representative.ID}]");

            return Created("api/v1/representatives/" + representative.ID, representativeDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]RepresentativeDTO representativeDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Representatives/api/v1 [PUT] was called", userId, token);

            if (!_representativeValidator.IsValid(representativeDTO))
            {
                _logger.LogError("{userId} {token} {status} {info}",
                    userId, token, StatusCodes.Status400BadRequest,
                    "RepresentativeDTO is invalid");

                return BadRequest();
            }

            var representative = await _unitOfWork.Representatives
                .GetById(representativeDTO.ID);

            if (representative == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Representative was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            representative.FullName.Name = representativeDTO.Name;
            representative.FullName.Patronymic = representativeDTO.Patronymic;
            representative.FullName.Surname = representativeDTO.Surname;
            representative.Birthday = representativeDTO.Birthday;
            representative.Rating = representativeDTO.Rating;
            representative.OrphanageID = representativeDTO.ChildrenHouseID;
            representative.EmailID = representativeDTO.EmailID;

            if (representativeDTO.Avatar != null)
            {
                _logger.LogInformation("{info}", "representativeDTO has file photo.");

                var fileName = representativeDTO.Name + representativeDTO.Surname
                    + representativeDTO.Patronymic + DateTime.Now.Ticks;

                representative.Avatar = _fileUploader.CopyFileToServer(fileName,
                    nameof(DirectoryUploadName.Representatives),
                    representativeDTO.Avatar);
            }

            _unitOfWork.Representatives.Update(representative);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                 token, userId, StatusCodes.Status204NoContent,
                 $"Representative was updated [id:{representative.ID}]");

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
               "Endpoint Representatives/api/v1 [DELETE] was called", userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            var representative = await _unitOfWork.Representatives.GetById(id);

            if (representative == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Representative was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            representative.IsDeleted = true;

            _unitOfWork.Representatives.Update(representative);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"Representative.IsDelete was updated [id:{id}]",
                userId, token);

            return Ok();
        }
    }
}