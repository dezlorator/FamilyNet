using FamilyNetServer.DTO;
using FamilyNetServer.Enums;
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
    public class LocationController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _repository;

        #endregion

        #region ctor

        public LocationController(IUnitOfWork repository)
        {
            _repository = repository;
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
                return BadRequest();
            }

            var locationDTO = new LocationDTO()
            {
                ID = locations.ID,
                MapCoordX = locations.MapCoordX,
                MapCoordY = locations.MapCoordY,

            };

            return Ok(locationDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]LocationDTO locationDTO)
        {
            
            var location = new Location()
            {
                MapCoordX = locationDTO.MapCoordX,
                MapCoordY = locationDTO.MapCoordY,
            };


            await _repository.Location.Create(location);
            _repository.SaveChangesAsync();

            locationDTO.ID = location.ID;

            return Created("api/v1/childrenHouse/" + locationDTO.ID, locationDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]LocationDTO locationDTO)
        {
            //if (!_childrenHouseValidator.IsValid(childrenHouseDTO))
            //{
            //    return BadRequest();
            //}

            var location = await _repository.Location.GetById(id);

            if (location == null)
            {
                return BadRequest();
            }

            location.MapCoordX = locationDTO.MapCoordX;
            location.MapCoordY = locationDTO.MapCoordY;

            _repository.Location.Update(location);
            _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var location = await _repository.Location.GetById(id);

            if (location == null)
            {
                return BadRequest();
            }

            location.IsDeleted = true;

            _repository.Location.Update(location);
            _repository.SaveChangesAsync();

            return Ok();
        }
    }
}
