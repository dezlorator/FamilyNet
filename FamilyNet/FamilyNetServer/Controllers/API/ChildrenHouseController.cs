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
    public class ChildrenHouseController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWorkAsync _repository;
        private readonly IFileUploader _fileUploader;
        private readonly IChildrenHouseValidator _childrenHouseValidator;
        private readonly IFilterConditionsChildrenHouse _filterConditions;

        #endregion

        #region ctor

        public ChildrenHouseController(IFileUploader fileUploader,
                                  IUnitOfWorkAsync repo,
                                  IChildrenHouseValidator childrenHouseValidator,
                                  IFilterConditionsChildrenHouse filterConditions)
        {
            _fileUploader = fileUploader;
            _repository = repo;
            _childrenHouseValidator = childrenHouseValidator;
            _filterConditions = filterConditions;
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
                    Rating = c.Rating,
                    PhotoPath = c.Avatar                    
                };

                if(c.Adress != null)
                {
                    childrenHouseDTO.Country = c.Adress.Country;
                    childrenHouseDTO.Region = c.Adress.Region;
                    childrenHouseDTO.City = c.Adress.City;
                    childrenHouseDTO.Street = c.Adress.Street;
                    childrenHouseDTO.House = c.Adress.House;
                }

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
                PhotoPath = childrenHouses.Avatar,
            };

            if (childrenHouses.Adress != null)
            {
                childrenHouseDTO.Country = childrenHouses.Adress.Country;
                childrenHouseDTO.Region = childrenHouses.Adress.Region;
                childrenHouseDTO.City = childrenHouses.Adress.City;
                childrenHouseDTO.Street = childrenHouses.Adress.Street;
                childrenHouseDTO.House = childrenHouses.Adress.House;
            }

            return Ok(childrenHouseDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]ChildrenHouseDTO childrenHousesDTO)
        {
            if (!_childrenHouseValidator.IsValid(childrenHousesDTO))
            {
                return BadRequest();
            }

            var address = new Address()
            {
                Country = childrenHousesDTO.Country,
                Region = childrenHousesDTO.Region,
                City = childrenHousesDTO.City,
                House = childrenHousesDTO.House,
                Street = childrenHousesDTO.Street
            };

            var pathPhoto = String.Empty;

            if (childrenHousesDTO.Avatar != null)
            {
                var fileName = childrenHousesDTO.Name + DateTime.Now.Ticks;

                pathPhoto = _fileUploader.CopyFile(fileName,
                        nameof(DirectoryUploadName.ChildrenHouses), childrenHousesDTO.Avatar);
            }

            var childrenHouse = new Orphanage()
            {
                Name = childrenHousesDTO.Name,
                AdressID = childrenHousesDTO.AdressID,
                Rating = childrenHousesDTO.Rating,
                Avatar = pathPhoto,
                Adress = address
            };

            bool IsLocationNotNull = GetCoordProp(address, out var Location);
            if (IsLocationNotNull)
            {
                childrenHouse.Location = new Location()
                {
                    MapCoordX = Location.Item1,
                    MapCoordY = Location.Item2
                };
                
            }
            else
                childrenHouse.LocationID = null;

            await _repository.Orphanages.Create(childrenHouse);
            _repository.SaveChangesAsync();

            childrenHousesDTO.ID = childrenHouse.ID;
            childrenHousesDTO.LocationID = childrenHouse.LocationID;
            childrenHousesDTO.AdressID = childrenHouse.AdressID;
            childrenHousesDTO.PhotoPath = childrenHouse.Avatar;
            childrenHousesDTO.Avatar = null;

            return Created("api/v1/childrenHouse/" + childrenHouse.ID, childrenHousesDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]ChildrenHouseDTO childrenHouseDTO)
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

            childrenHouse.Adress.Country = childrenHouseDTO.Country;
            childrenHouse.Adress.Region = childrenHouseDTO.Region;
            childrenHouse.Adress.City = childrenHouseDTO.City;
            childrenHouse.Adress.Street = childrenHouseDTO.Street;
            childrenHouse.Adress.House = childrenHouseDTO.House;

            bool IsLocationNotNull = GetCoordProp(childrenHouse.Adress, out var Location);
            if (IsLocationNotNull)
            {
                childrenHouse.Location = new Location()
                {
                    MapCoordX = Location.Item1,
                    MapCoordY = Location.Item2
                };

            }
            else
                childrenHouse.LocationID = null;

            childrenHouse.Name = childrenHouseDTO.Name;
            childrenHouse.Rating = childrenHouseDTO.Rating;

            if (childrenHouseDTO.Avatar != null)
            {
                var fileName = childrenHouseDTO.Name + DateTime.Now.Ticks;

                childrenHouse.Avatar = _fileUploader.CopyFile(fileName,
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
            childrenHouse.Adress.IsDeleted = true;
            childrenHouse.Location.IsDeleted = true;

            _repository.Orphanages.Update(childrenHouse);
            _repository.SaveChangesAsync();

            return Ok();
        }

        private bool GetCoordProp(Address address, out Tuple<float?, float?> result)
        {
            result = null;
            bool forOut = false;

            var nominatim = new Nominatim.API.Geocoders.ForwardGeocoder();
            var d = nominatim.Geocode(new Nominatim.API.Models.ForwardGeocodeRequest()
            {
                Country = address.Country,
                State = address.Region,
                City = address.City,
                StreetAddress = String.Concat(address.Street, " ", address.House)
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
