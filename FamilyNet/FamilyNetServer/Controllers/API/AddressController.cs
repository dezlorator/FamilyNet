using FamilyNetServer.DTO;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
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
    public class AddressController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWorkAsync _repository;

        #endregion

        #region ctor

        public AddressController(IUnitOfWorkAsync repo)
        {            
            _repository = repo;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll()
        {
            var address = _repository.Address.GetAll().Where(c => !c.IsDeleted);
          
            if (address == null)
            {
                return BadRequest();
            }

            var addressDTO = new List<AddressDTO>();

            foreach (var adrs in address)
            {
                var childrenHouseDTO = new AddressDTO()
                {
                    ID = adrs.ID,
                    Country = adrs.Country,
                    Region = adrs.Region,
                    City = adrs.City,
                    House = adrs.House,
                    Street = adrs.Street
                };
         
                addressDTO.Add(childrenHouseDTO);
            }

            return Ok(addressDTO);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var addresses = await _repository.Address.GetById(id);

            if (addresses == null)
            {
                return BadRequest();
            }

            var addressDTO = new AddressDTO()
            {
                ID = addresses.ID,
                Country = addresses.Country,
                Region = addresses.Region,
                City = addresses.City,
                Street = addresses.Street,
                House = addresses.House
            };

            return Ok(addressDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]AddressDTO addressDTO)
        {
            //if (!_childrenHouseValidator.IsValid(childrenHousesDTO))
            //{
            //    return BadRequest();
            //}

            var address = new Address()
            {
                Country = addressDTO.Country,
                Region = addressDTO.Region,
                City = addressDTO.City,
                House = addressDTO.House,
                Street = addressDTO.Street
            };

           
            await _repository.Address.Create(address);
            _repository.SaveChangesAsync();

            addressDTO.ID = address.ID;

            return Created(addressDTO.ID.ToString(), addressDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]AddressDTO addressDTO)
        {
            //if (!_childrenHouseValidator.IsValid(childrenHouseDTO))
            //{
            //    return BadRequest();
            //}

            var address = await _repository.Address.GetById(id);

            if (address == null)
            {
                return BadRequest();
            }

            address.Country = addressDTO.Country;
            address.Region = addressDTO.Region;
            address.City = addressDTO.City;
            address.Street = addressDTO.Street;
            address.House = addressDTO.House;
       
            _repository.Address.Update(address);
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

            var address = await _repository.Address.GetById(id);

            if (address == null)
            {
                return BadRequest();
            }

            address.IsDeleted = true;

            _repository.Address.Update(address);
            _repository.SaveChangesAsync();

            return Ok();
        }
    }
}
