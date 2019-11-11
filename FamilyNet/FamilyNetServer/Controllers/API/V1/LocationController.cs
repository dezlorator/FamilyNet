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
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
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
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("{info}",
                "Endpoint Location/api/v1 GetAll was called");

            var location = _repository.Location.GetAll()
                .Where(c => !c.IsDeleted);

            if (location == null)
            {
                _logger.LogInformation("{status}{info}",
                    StatusCodes.Status400BadRequest,
                    "List of Locations is empty");

                return BadRequest();
            }

            var locationsDTO = await location.Select(Location =>
                new LocationDTO()
                {
                    ID = Location.ID,
                    MapCoordX = Location.MapCoordX,
                    MapCoordY = Location.MapCoordY,
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
                 $"Endpoint Location/api/v1 GetById({id}) was called");

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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]AddressDTO addressDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Location/api/v1 [POST] was called", userId, token);

            if (!_addressValidator.IsValid(addressDTO))
            {
                _logger.LogWarning("{status}{token}{userId}",
                    StatusCodes.Status400BadRequest,
                    token, userId);

                return BadRequest();
            }

            bool IsLocationNotNull = GetCoordProp(addressDTO, out var coord);
            Location location = null;

            if (IsLocationNotNull)
            {
                location = new Location()
                {
                    MapCoordX = coord.Item1,
                    MapCoordY = coord.Item2,
                };
            }
            else
            {
                _logger.LogError("{status}{token}{userId}{info}",
                    StatusCodes.Status400BadRequest, token, userId,
                    "Invalid address data");

                return BadRequest();
            }

            await _repository.Location.Create(location);
            _repository.SaveChangesAsync();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Location was saved [id:{location.ID}]");

            return Created("api/v1/childrenHouse/" + location.ID, location);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromForm]AddressDTO addressDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Location/api/v1 [PUT] was called", userId, token);

            if (!_addressValidator.IsValid(addressDTO))
            {
                _logger.LogError("{userId} {token} {status} {info}", userId,
                    token, StatusCodes.Status400BadRequest,
                    "Address data is invalid");

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

            bool IsLocationNotNull = GetCoordProp(addressDTO, out var coord);
            if (IsLocationNotNull)
            {
                location.MapCoordX = coord.Item1;
                location.MapCoordY = coord.Item2;
            }
            else
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest, 
                    $"Address data was not found [id:{id}]", userId, token);

                location.IsDeleted = true;
            }
            _repository.Location.Update(location);
            _repository.SaveChangesAsync();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status204NoContent,
                $"Location was updated [id:{location.ID}]");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
               "Endpoint Location/api/v1 [DELETE] was called", userId, token);

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
            _repository.SaveChangesAsync();


            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"Location.IsDelete was updated [id:{id}]", userId, token);

            return Ok();
        }

        private bool GetCoordProp(AddressDTO addressDTO, out Tuple<float?, float?> result)
        {
            result = null;
            bool forOut = false;

            var nominatim = new Nominatim.API.Geocoders.ForwardGeocoder();
            var d = nominatim.Geocode(new Nominatim.API.Models.ForwardGeocodeRequest()
            {
                Country = addressDTO.Country,
                State = addressDTO.Region,
                City = addressDTO.City,
                StreetAddress = String.Concat(addressDTO.Street, " ", addressDTO.House)
            });

            //TODO:some validation for search
            if (d != null)
            {
                if (d.Result.Count() != 0)
                {
                    float? X = (float)d.Result[0].Latitude;
                    float? Y = (float)d.Result[0].Longitude;

                    result = new Tuple<float?, float?>(X, Y);
                    forOut = true;
                }
            }

            return forOut;
        }
    }
}
