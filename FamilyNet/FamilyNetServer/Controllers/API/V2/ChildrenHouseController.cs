using DataTransferObjects;
using FamilyNetServer.Configuration;
using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API.V2
{
    [Route("api/v2/[controller]")]
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

        #endregion

        #region ctor

        public ChildrenHouseController(IFileUploader fileUploader,
                                  IUnitOfWork repo,
                                  IValidator<ChildrenHouseDTO> childrenHouseValidator,
                                  IFilterConditionsChildrenHouse filterConditions,
                                  IOptionsSnapshot<ServerURLSettings> setings,
                                  ILogger<ChildrenHouseController> logger)
        {
            _fileUploader = fileUploader;
            _repository = repo;
            _childrenHouseValidator = childrenHouseValidator;
            _filterConditions = filterConditions;
            _settings = setings;
            _logger = logger;
        }

        #endregion

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]string name,
                                   [FromQuery]float rating,
                                   [FromQuery]string address,
                                   [FromQuery]string sort,
                                   [FromQuery]int rows,
                                   [FromQuery]int page)
        {
            var childrenHouses = _repository.Orphanages.GetAll().Where(c => !c.IsDeleted);
            _logger.LogInformation("Get all children houses");
            childrenHouses = _filterConditions.GetFilteredChildrenHouses(childrenHouses, name, rating, address);
            _logger.LogInformation("Get children houses filtered");
            childrenHouses = _filterConditions.GetSortedChildrenHouses(childrenHouses, sort);
            _logger.LogInformation("Get children houses sorted");

            if (rows != 0 && page != 0)
            {
                childrenHouses = childrenHouses.Skip((page - 1) * rows).Take(rows);
            }

            if (childrenHouses == null)
            {
                _logger.LogError("No children houses in database");
                return BadRequest();
            }

            var childrenDTO = new List<ChildrenHouseDTO>();

            foreach (var c in childrenHouses)
            {
                var childrenHouseDTO = new ChildrenHouseDTO()
                {
                    ID = c.ID,
                    Name = c.Name,
                    AdressID = c.AdressID,
                    LocationID = c.LocationID,
                    Rating = c.Rating,
                    PhotoPath = _settings.Value.ServerURL + c.Avatar,
                };

                childrenDTO.Add(childrenHouseDTO);
            }

            _logger.LogInformation("Returned children house list");
            return Ok(childrenDTO);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var childrenHouses = await _repository.Orphanages.GetById(id);

            if (childrenHouses == null)
            {
                _logger.LogError($"No children house with #{id} in database");
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

            _logger.LogInformation($"Returned children house with id #{id}");
            return Ok(childrenHouseDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        public async Task<IActionResult> Create([FromBody]ChildrenHouseDTO childrenHousesDTO)
        {
            if (!_childrenHouseValidator.IsValid(childrenHousesDTO))
            {
                _logger.LogError("Invalid children house");
                return BadRequest();
            }
            
            var pathPhoto = String.Empty;

            if (childrenHousesDTO.Avatar != null)
            {
                var fileName = childrenHousesDTO.Name + DateTime.Now.Ticks;

                pathPhoto = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.ChildrenHouses), childrenHousesDTO.Avatar);
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

            childrenHousesDTO.ID = childrenHouse.ID;
            childrenHousesDTO.PhotoPath = childrenHouse.Avatar;
            childrenHousesDTO.Avatar = null;

            _logger.LogInformation($"Created children house with id #{childrenHouse.ID}");

            return Created("api/v1/childrenHouse/" + childrenHouse.ID, childrenHousesDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]ChildrenHouseDTO childrenHouseDTO)
        {
            if (!_childrenHouseValidator.IsValid(childrenHouseDTO))
            {
                _logger.LogError("Invalid children house");
                return BadRequest();
            }

            var childrenHouse = await _repository.Orphanages.GetById(id);

            if (childrenHouse == null)
            {
                return BadRequest();
            }

            childrenHouse.Name = childrenHouseDTO.Name;
            childrenHouse.Rating = childrenHouseDTO.Rating;
            childrenHouse.LocationID = childrenHouseDTO.LocationID;

            if (childrenHouseDTO.Avatar != null)
            {
                var fileName = childrenHouseDTO.Name + DateTime.Now.Ticks;

                childrenHouse.Avatar = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.ChildrenHouses), childrenHouseDTO.Avatar);
            }

            _repository.Orphanages.Update(childrenHouse);
            _repository.SaveChanges();

            _logger.LogInformation($"Edited children house with id #{childrenHouse.ID}");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Adminб Representative")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Invalid id");
                return BadRequest();
            }

            var childrenHouse = await _repository.Orphanages.GetById(id);

            if (childrenHouse == null)
            {
                _logger.LogError($"No children house with #{id} in database");
                return BadRequest();
            }

            childrenHouse.IsDeleted = true;

            _repository.Orphanages.Update(childrenHouse);
            _repository.SaveChanges();

            _logger.LogInformation($"Deleted children house with id #{childrenHouse.ID}");

            return Ok();
        }
    }
}
