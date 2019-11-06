using DataTransferObjects;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _repository;
        private readonly IValidator<AddressDTO> _addressValidator;
        private readonly ILogger<AddressController> _logger;


        #endregion

        #region ctor

        public AddressController(IUnitOfWork repo, IValidator<AddressDTO> addressValidator,
            ILogger<AddressController> logger)
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
            var address = _repository.Address.GetAll().Where(c => !c.IsDeleted);

            if (address == null)
            {
                _logger.LogError("No address in database");
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

            _logger.LogInformation("Returned address list");
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
                _logger.LogError($"No address with id #{id} in database");
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

            _logger.LogInformation($"Returned address #{id}");
            return Ok(addressDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]AddressDTO addressDTO)
        {
            if (!_addressValidator.IsValid(addressDTO))
            {
                _logger.LogError("Invalid address");
                return BadRequest();
            }

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

            _logger.LogInformation($"Created address with id #{address.ID}");

            return Created(addressDTO.ID.ToString(), addressDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromForm]AddressDTO addressDTO)
        {
            if (!_addressValidator.IsValid(addressDTO))
            {
                _logger.LogError("Invalid address");
                return BadRequest();
            }

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
            _logger.LogInformation($"Edited address with id #{address.ID}");

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
                _logger.LogError("No address in database");
                return BadRequest();
            }

            var address = await _repository.Address.GetById(id);

            if (address == null)
            {
                _logger.LogError("No address in database");
                return BadRequest();
            }

            address.IsDeleted = true;

            _repository.Address.Update(address);
            _repository.SaveChangesAsync();

            _logger.LogInformation($"Deleted address with id #{address.ID}");

            return Ok();
        }
    }
}
