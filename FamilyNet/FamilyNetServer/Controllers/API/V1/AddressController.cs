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
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public AddressController(IUnitOfWork repo,
                                 IValidator<AddressDTO> addressValidator,
                                 ILogger<AddressController> logger,
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
                "Endpoint Address/api/v1 GetAll was called");

            var address = _repository.Address.GetAll().Where(c => !c.IsDeleted);

            if (address == null)
            {
                _logger.LogWarning("{status}{info}",
                    StatusCodes.Status400BadRequest,
                    "List of addresses is empty");

                return BadRequest();
            }

            var addressDTO = await address.Select(adrs =>
                new AddressDTO()
                {
                    ID = adrs.ID,
                    Country = adrs.Country,
                    Region = adrs.Region,
                    City = adrs.City,
                    House = adrs.House,
                    Street = adrs.Street
                }).ToListAsync();

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(addressDTO));

            return Ok(addressDTO);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                  $"Endpoint Address/api/v1 GetById({id}) was called");

            var addresses = await _repository.Address.GetById(id);

            if (addresses == null)
            {
                _logger.LogError("{info}{status}",
                    $"Address wasn't found. [id:{id}]",
                    StatusCodes.Status400BadRequest);

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

            _logger.LogInformation("{status},{json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(addressDTO));

            return Ok(addressDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Representative, CharityMaker, Volunteer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]AddressDTO addressDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Assress/api/v1 [POST] was called", userId, token);

            if (!_addressValidator.IsValid(addressDTO))
            {
                _logger.LogWarning("{status}{token}{userId}",
                    StatusCodes.Status400BadRequest,
                    token, userId);

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
            _repository.SaveChanges();

            addressDTO.ID = address.ID;

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Address was saved [id:{address.ID}]");

            return Created(addressDTO.ID.ToString(), addressDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Representative, CharityMaker, Volunteer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromForm]AddressDTO addressDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Address/api/v1 [PUT] was called", userId, token);

            if (!_addressValidator.IsValid(addressDTO))
            {
                _logger.LogError("{userId}{token}{status}{info}",
                   userId, token, StatusCodes.Status400BadRequest,
                   "AddressDTO is invalid");

                return BadRequest();
            }

            var address = await _repository.Address.GetById(id);

            if (address == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                   StatusCodes.Status400BadRequest, "Address was not found",
                   userId, token);

                return BadRequest();
            }

            address.Country = addressDTO.Country;
            address.Region = addressDTO.Region;
            address.City = addressDTO.City;
            address.Street = addressDTO.Street;
            address.House = addressDTO.House;

            _repository.Address.Update(address);
            _repository.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                 token, userId, StatusCodes.Status204NoContent,
                 $"Address was updated [id:{address.ID}]");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Representative, CharityMaker, Volunteer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
               "Endpoint Address/api/v1 [DELETE] was called", userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                  StatusCodes.Status400BadRequest,
                  $"Argument id is not valid [id:{id}]",
                  userId, token);

                return BadRequest();
            }

            var address = await _repository.Address.GetById(id);

            if (address == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Address was not found [id:{id}]",
                    userId, token);

                return BadRequest();
            }

            address.IsDeleted = true;

            _repository.Address.Update(address);
            _repository.SaveChanges();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"Address.IsDelete was updated [id:{id}]",
                userId, token);

            return Ok();
        }
    }
}
