using DataTransferObjects;
using FamilyNetServer.Configuration;
using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.HttpHandlers;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ChildrenHouseController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _repository;
        private readonly IFileUploader _fileUploader;
        private readonly IValidator<ChildrenHouseDTO> _childrenHouseValidator;
        private readonly IFilterConditionsChildrenHouse _filterConditions;
        private readonly IOptionsSnapshot<ServerURLSettings> _settings;
        private readonly ILogger<ChildrenHouseController> _logger;
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public ChildrenHouseController(IFileUploader fileUploader,
                                  IUnitOfWork repo,
                                  IValidator<ChildrenHouseDTO> childrenHouseValidator,
                                  IFilterConditionsChildrenHouse filterConditions,
                                  IOptionsSnapshot<ServerURLSettings> setings,
                                  ILogger<ChildrenHouseController> logger,
                                  IIdentityExtractor identityExtractor)

        {
            _fileUploader = fileUploader;
            _repository = repo;
            _childrenHouseValidator = childrenHouseValidator;
            _filterConditions = filterConditions;
            _settings = setings;
            _logger = logger;
            _identityExtractor = identityExtractor;
        }

        #endregion

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery]string name,
                                   [FromQuery]float rating,
                                   [FromQuery]string address,
                                   [FromQuery]string sort,
                                   [FromQuery]int rows,
                                   [FromQuery]int page)
        {
            _logger.LogInformation("{info}",
                "Endpoint ChildrenHouses/api/v1 GetAll was called");

            var childrenHouses = _repository.Orphanages.GetAll()
                .Where(c => !c.IsDeleted);
            _logger.LogInformation("Get all children houses");
            childrenHouses = _filterConditions
                .GetFilteredChildrenHouses(childrenHouses, name, rating, address);
            _logger.LogInformation("{info}", "Get children houses filtered");
            childrenHouses = _filterConditions
                .GetSortedChildrenHouses(childrenHouses, sort);
            _logger.LogInformation("{info}", "Get children houses sorted");

            if (rows > 0 && page > 0)
            {
                _logger.LogInformation("{info}", "Paging was used");
                childrenHouses = childrenHouses.Skip((page - 1) * rows)
                    .Take(rows);
            }

            if (childrenHouses == null)
            {
                _logger.LogInformation("{status}{info}",
                    StatusCodes.Status400BadRequest,
                    "List of Children Houses is empty");

                return BadRequest();
            }

            var childrenHousesDTO = await childrenHouses.Select(c =>
                new ChildrenHouseDTO()
                {
                    ID = c.ID,
                    Name = c.Name,
                    AdressID = c.AdressID,
                    LocationID = c.LocationID,
                    Rating = c.Rating,
                    PhotoPath = _settings.Value.ServerURL + c.Avatar,
                }).ToListAsync();

            _logger.LogInformation("{status}{json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(childrenHousesDTO));

            return Ok(childrenHousesDTO);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                $"Endpoint ChildrenHouses/api/v1 GetById({id}) was called");

            var childrenHouses = await _repository.Orphanages.GetById(id);

            if (childrenHouses == null)
            {
                _logger.LogError("{info}{status}",
                    $"ChildrenHouses wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var childrenHouseDTO = new ChildrenHouseDTO()
            {
                ID = childrenHouses.ID,
                Name = childrenHouses.Name,
                AdressID = childrenHouses.AdressID,
                Rating = childrenHouses.Rating,
                LocationID = childrenHouses.LocationID,
                PhotoPath = _settings.Value.ServerURL + childrenHouses.Avatar
            };

            _logger.LogInformation("{status}{json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(childrenHouseDTO));

            return Ok(childrenHouseDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]ChildrenHouseDTO childrenHousesDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint ChildrenHouses/api/v1 [POST] was called",
                userId, token);

            if (!_childrenHouseValidator.IsValid(childrenHousesDTO))
            {
                _logger.LogWarning("{status}{token}{userId}",
                   StatusCodes.Status400BadRequest, token, userId);

                return BadRequest();
            }

            var pathPhoto = String.Empty;

            if (childrenHousesDTO.Avatar != null)
            {
                _logger.LogInformation("{info}", "ChildrenHouseDTO has file photo.");

                var fileName = childrenHousesDTO.Name + DateTime.Now.Ticks;

                pathPhoto = _fileUploader.CopyFileToServer(fileName,
                    nameof(DirectoryUploadName.ChildrenHouses),
                    childrenHousesDTO.Avatar);
            }

            var childrenHouse = new Orphanage()
            {
                Name = childrenHousesDTO.Name,
                AdressID = childrenHousesDTO.AdressID,
                LocationID = childrenHousesDTO.LocationID,
                Rating = childrenHousesDTO.Rating,
                Avatar = pathPhoto,
            };

            await _repository.Orphanages.Create(childrenHouse);
            _repository.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"ChildHouses was saved [id:{childrenHouse.ID}]");

            childrenHousesDTO.ID = childrenHouse.ID;
            childrenHousesDTO.PhotoPath = childrenHouse.Avatar;
            childrenHousesDTO.Avatar = null;

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Children House was saved [id:{childrenHouse.ID}]");

            return Created("api/v1/childrenHouse/" + childrenHouse.ID, childrenHousesDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromForm]ChildrenHouseDTO childrenHouseDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint ChildrenHouses/api/v1 [PUT] was called", userId, token);

            if (!_childrenHouseValidator.IsValid(childrenHouseDTO))
            {
                _logger.LogError("{userId} {token} {status} {info}", userId,
                    token, StatusCodes.Status400BadRequest,
                    "ChildHouseDTO is invalid");

                return BadRequest();
            }

            var childrenHouse = await _repository.Orphanages.GetById(id);

            if (childrenHouse == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"ChildldrenHouse was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            childrenHouse.Name = childrenHouseDTO.Name;
            childrenHouse.Rating = childrenHouseDTO.Rating;
            childrenHouse.LocationID = childrenHouseDTO.LocationID;

            if (childrenHouseDTO.Avatar != null)
            {
                _logger.LogInformation("{info}",
                    "ChildrenHouseDTO has file photo.");

                var fileName = childrenHouseDTO.Name + DateTime.Now.Ticks;

                childrenHouse.Avatar = _fileUploader.CopyFileToServer(fileName,
                    nameof(DirectoryUploadName.ChildrenHouses),
                    childrenHouseDTO.Avatar);
            }

            _repository.Orphanages.Update(childrenHouse);
            _repository.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status204NoContent,
                $"ChildrenHouse was updated [id:{childrenHouse.ID}]");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint ChildrenHouse/api/v1 [DELETE] was called",
                userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                   StatusCodes.Status400BadRequest,
                   $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            var childrenHouse = await _repository.Orphanages.GetById(id);

            if (childrenHouse == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"ChildrenHouse was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            childrenHouse.IsDeleted = true;

            _repository.Orphanages.Update(childrenHouse);
            _repository.SaveChanges();

            _logger.LogInformation("{status} {info} {userId} {token}",
                 StatusCodes.Status200OK,
                 $"ChildrenHouse.IsDelete was updated [id:{id}]", 
                 userId, token);

            return Ok();
        }
    }
}
