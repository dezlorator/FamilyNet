using FamilyNetServer.DTO;
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
            var childrenHouses = _repository.Orphanages.GetAll().Where(c => !c.IsDeleted && c.LocationID!=null);
           
            if (childrenHouses == null)
            {
                return BadRequest();
            }

            var locationDTO = new List<LocationDTO>();

            foreach (var c in childrenHouses)
            {
                var childDTO = new LocationDTO()
                {
                    ID = c.Location.ID,
                    MapCoordX = c.Location.MapCoordX,
                    MapCoordY = c.Location.MapCoordY,
                    ChildrenHouseName = c.Name
                };

                locationDTO.Add(childDTO);
            }

            return Ok(locationDTO);
        }
    }
}
