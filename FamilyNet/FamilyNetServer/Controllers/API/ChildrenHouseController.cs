using DataTransferObjects;
using FamilyNetServer.Configuration;
using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API
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

        #endregion

        #region ctor

        public ChildrenHouseController(IFileUploader fileUploader,
                                  IUnitOfWork repo,
                                  IValidator<ChildrenHouseDTO> childrenHouseValidator,
                                  IFilterConditionsChildrenHouse filterConditions,
                                  IOptionsSnapshot<ServerURLSettings> setings)
        {
            _fileUploader = fileUploader;
            _repository = repo;
            _childrenHouseValidator = childrenHouseValidator;
            _filterConditions = filterConditions;
            _settings = setings;
        }

        #endregion

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
            childrenHouses = _filterConditions.GetFilteredChildrenHouses(childrenHouses, name, rating, address);
            childrenHouses = _filterConditions.GetSortedChildrenHouses(childrenHouses, sort);

            if (rows != 0 && page != 0)
            {
                childrenHouses = childrenHouses.Skip((page - 1) * rows).Take(rows);
            }

            if (childrenHouses == null)
            {
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

            return Ok(childrenDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var childrenHouses = await _repository.Orphanages.GetById(id);

            if (childrenHouses == null)
            {
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

            return Ok(childrenHouseDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]ChildrenHouseDTO childrenHousesDTO)
        {
            if (!_childrenHouseValidator.IsValid(childrenHousesDTO))
            {
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
            _repository.SaveChangesAsync();

            childrenHousesDTO.ID = childrenHouse.ID;
            childrenHousesDTO.PhotoPath = childrenHouse.Avatar;
            childrenHousesDTO.Avatar = null;

            return Created("api/v1/childrenHouse/" + childrenHouse.ID, childrenHousesDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromForm]ChildrenHouseDTO childrenHouseDTO)
        {
            if (!_childrenHouseValidator.IsValid(childrenHouseDTO))
            {
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

            var childrenHouse = await _repository.Orphanages.GetById(id);

            if (childrenHouse == null)
            {
                return BadRequest();
            }

            childrenHouse.IsDeleted = true;

            _repository.Orphanages.Update(childrenHouse);
            _repository.SaveChangesAsync();

            return Ok();
        }

    }
}
