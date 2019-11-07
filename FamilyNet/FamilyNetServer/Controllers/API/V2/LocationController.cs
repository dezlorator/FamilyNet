using DataTransferObjects;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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

        #endregion

        #region ctor

        public LocationController(IUnitOfWork repo, IValidator<AddressDTO> addressValidator,
            ILogger<LocationController> logger)
        {
            _repository = repo;
            _addressValidator = addressValidator;
            _logger = logger;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll()
        {
            var location = _repository.Location.GetAll().Where(c => !c.IsDeleted);

            if (location == null)
            {
                _logger.LogError("No location in database");
                return BadRequest();
            }

            var locationsDTO = new List<LocationDTO>();

            foreach (var c in location)
            {
                var locationDTO = new LocationDTO()
                {
                    ID = c.ID,
                    MapCoordX = c.MapCoordX,
                    MapCoordY = c.MapCoordY,
                };

                locationsDTO.Add(locationDTO);
            }

            _logger.LogInformation("Returned location list");

            return Ok(locationsDTO);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var locations = await _repository.Location.GetById(id);

            if (locations == null)
            {
                _logger.LogError($"No location with id #{id} in database");
                return BadRequest();
            }

            var locationDTO = new LocationDTO()
            {
                ID = locations.ID,
                MapCoordX = locations.MapCoordX,
                MapCoordY = locations.MapCoordY,

            };

            _logger.LogInformation($"Returned location with id #{id}");
            return Ok(locationDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]AddressDTO addressDTO)
        {
            if (!_addressValidator.IsValid(addressDTO))
            {
                _logger.LogError("Invalid address data");
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
                _logger.LogError("Invalid address data");
                return BadRequest();
            }

            await _repository.Location.Create(location);
            _repository.SaveChangesAsync();

            _logger.LogInformation($"Created location with id #{location.ID}");
            
            return Created("api/v1/childrenHouse/" + location.ID, location);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]AddressDTO addressDTO)
        {
            if (!_addressValidator.IsValid(addressDTO))
            {
                _logger.LogError("Invalid address data");
                return BadRequest();
            }

            var location = await _repository.Location.GetById(id);
            if (location == null)
            {
                _logger.LogError($"No location with id #{id} in database");
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
                _logger.LogInformation("Invalid address data");
                location.IsDeleted = true;
            }
            _repository.Location.Update(location);
            _repository.SaveChangesAsync();

            _logger.LogInformation($"Edited location with id #{location.ID}");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid id");
                return BadRequest();
            }

            var location = await _repository.Location.GetById(id);

            if (location == null)
            {
                _logger.LogError($"No location with id #{id} in database");
                return BadRequest();
            }

            location.IsDeleted = true;

            _repository.Location.Update(location);
            _repository.SaveChangesAsync();

            _logger.LogInformation($"Deleted location with id #{location.ID}");

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
