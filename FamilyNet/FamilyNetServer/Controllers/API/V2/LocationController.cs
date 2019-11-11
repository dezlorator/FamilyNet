using DataTransferObjects;
using FamilyNetServer.HttpHandlers;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _repository;
        private readonly IValidator<AddressDTO> _addressValidator;
        private readonly ILogger<LocationController> _logger;
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public LocationController(IUnitOfWork repo,
                                  IValidator<AddressDTO> addressValidator,
                                  ILogger<LocationController> logger,
                                  IIdentityExtractor identityExtractor)
        {
            _repository = repo;
            _addressValidator = addressValidator;
            _logger = logger;
            _identityExtractor = identityExtractor;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllAsync()
        {
            _logger.LogInformation("{info}",
               "Endpoint Location/api/v2 GetAll was called");

            var location = _repository.Location.GetAll().Where(c => !c.IsDeleted);

            if (location == null)
            {
                _logger.LogInformation("{status}{info}",
                                   StatusCodes.Status400BadRequest,
                                   "List of Locations is empty");

                return BadRequest();
            }

            var locationsDTO = await location.Select(c =>
             new LocationDTO()
             {
                 ID = c.ID,
                 MapCoordX = c.MapCoordX,
                 MapCoordY = c.MapCoordY,
             }).ToListAsync();

            _logger.LogInformation("{status}, {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(locationsDTO));

            return Ok(locationsDTO);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                $"Endpoint Location/api/v2 GetById({id}) was called");

            var locations = await _repository.Location.GetById(id);

            if (locations == null)
            {
                _logger.LogError("{info}{status}",
                    $"Location wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var locationDTO = new LocationDTO()
            {
                ID = locations.ID,
                MapCoordX = locations.MapCoordX,
                MapCoordY = locations.MapCoordY,

            };

            _logger.LogInformation("{status},{json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(locationDTO));

            return Ok(locationDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody]LocationDTO locationDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Location/api/v1 [POST] was called", userId, token);

            if (locationDTO == null)
            {
                _logger.LogWarning("{status}{token}{userId}",
                    StatusCodes.Status400BadRequest,
                    token, userId);

                return BadRequest();
            }

            if (!locationDTO.IsValid)
            {
                _logger.LogInformation("{info}{status}", "Deleted location",
                    StatusCodes.Status404NotFound);

                return NotFound();
            }

            var loc = new Location
            {
                MapCoordX = locationDTO.MapCoordX,
                MapCoordY = locationDTO.MapCoordY,
            };

            await _repository.Location.Create(loc);
            _repository.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Location was saved [id:{loc.ID}]");

            return Created("api/v1/locaiton/" + loc.ID, loc);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]LocationDTO locationDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Location/api/v2 [PUT] was called", userId, token);

            if (locationDTO == null)
            {
                _logger.LogError("{userId} {token} {status} {info}", userId,
                    token, StatusCodes.Status400BadRequest,
                    "AddressDTO is null");

                return BadRequest();
            }

            var location = await _repository.Location.GetById(id);

            if (location == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Location was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            if (!locationDTO.IsValid)
            {
                _logger.LogError("{userId} {token} {status} {info}", userId,
                    token, StatusCodes.Status400BadRequest,
                    "Address data is invalid");

                location.IsDeleted = true;
                return NotFound();
            }

            _repository.Location.Update(location);
            _repository.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status204NoContent,
                $"Location was updated [id:{location.ID}]");

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
               "Endpoint Location/api/v2 [DELETE] was called", userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            var location = await _repository.Location.GetById(id);

            if (location == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Location was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            location.IsDeleted = true;

            _repository.Location.Update(location);
            _repository.SaveChanges();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"Location.IsDelete was updated [id:{id}]", userId, token);

            return Ok();
        }
    }
}
