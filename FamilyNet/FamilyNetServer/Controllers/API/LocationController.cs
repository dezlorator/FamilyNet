﻿using FamilyNetServer.DTO;
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
    public class LocationController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWorkAsync _repository;

        #endregion

        #region ctor

        public LocationController(IUnitOfWorkAsync repository)
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
        public async Task<IActionResult> Create([FromBody]AddressDTO addressDTO)
        {
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
                return BadRequest();

            await _repository.Location.Create(location);
            _repository.SaveChangesAsync();

            return Created("api/v1/childrenHouse/" + location.ID, location);
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
            if (d.Result.Count() != 0)
            {
                float? X = (float)d.Result[0].Latitude;
                float? Y = (float)d.Result[0].Longitude;

                result = new Tuple<float?, float?>(X, Y);
                forOut = true;
            }

            return forOut;
        }
    }
}
