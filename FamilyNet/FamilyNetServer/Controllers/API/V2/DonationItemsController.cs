using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.Models;
using FamilyNetServer.Validators;
using DataTransferObjects;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace FamilyNetServer.Controllers.API.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class DonationItemsController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DonationItemDTO> _donationItemValidator;
        private readonly ILogger<DonationItemsController> _logger;

        #endregion

        public DonationItemsController(IUnitOfWork unitOfWork,
                                  IValidator<DonationItemDTO> donationItemValidator,
                                  ILogger<DonationItemsController> logger)
        {
            _unitOfWork = unitOfWork;
            _donationItemValidator = donationItemValidator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll()
        {
            var donationItems = _unitOfWork.DonationItems.GetAll().Where(b => !b.IsDeleted);

            if (donationItems == null)
            {
                _logger.LogError("Bad request. No donation items were found");
                return BadRequest();
            }

            var donationItemsDTO = new List<DonationItemDTO>();

            donationItemsDTO = donationItems.Select(d =>
                new DonationItemDTO
                {
                    ID = d.ID,
                    Name = d.Name,
                    Description = d.Description,
                    Price = d.Price,
                    CategoriesID = d.TypeBaseItem.Select(t => t.TypeID)
                }).ToList();

            _logger.LogInformation("Status: OK. List of donation items was sent");

            return Ok(donationItemsDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var donationItem = await _unitOfWork.DonationItems.GetById(id);

            if (donationItem == null)
            {
                _logger.LogError("Bad request. No donation item was found");
                return BadRequest();
            }

            var donationItemDTO = new DonationItemDTO()
            {
                ID = donationItem.ID,
                Name = donationItem.Name,
                Description = donationItem.Description,
                Price = donationItem.Price,
                CategoriesID = donationItem.TypeBaseItem.Select(t => t.TypeID)
            };

            _logger.LogInformation("Status: OK. Donation item was sent");
            return Ok(donationItemDTO);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Volunteer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]DonationItemDTO donationItemDTO)
        {
            if (!_donationItemValidator.IsValid(donationItemDTO))
            {
                _logger.LogError("Model is not valid.");
                return BadRequest();
            }

            var donationItem = new DonationItem()
            {
                Name = donationItemDTO.Name,
                Description = donationItemDTO.Description,
                Price = donationItemDTO.Price
            };

            int ID = donationItem.ID;

            donationItem.TypeBaseItem = new List<TypeBaseItem>();

            if (donationItemDTO.CategoriesID != null)
            {
                donationItem.TypeBaseItem = donationItemDTO.CategoriesID.Select(c =>
                    new TypeBaseItem
                    {
                        ItemID = ID,
                        TypeID = c
                    }
                ).ToList();

                _logger.LogInformation("Categories were added.");
            }

            await _unitOfWork.DonationItems.Create(donationItem);
            _unitOfWork.SaveChanges();

            donationItemDTO.ID = donationItem.ID;

            _logger.LogInformation("Status: Created. Donation was created");

            return Created("api/v1/donationItems/" + donationItem.ID, donationItemDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Volunteer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromBody]DonationItemDTO donationItemDTO)
        {
            if (!_donationItemValidator.IsValid(donationItemDTO))
            {
                _logger.LogError("Model is not valid.");
                return BadRequest();
            }

            var donationItem = await _unitOfWork.DonationItems.GetById(id);

            if (donationItem == null)
            {
                _logger.LogError("Bad request. No donation item was found");
                return BadRequest();
            }

            donationItem.Name = donationItemDTO.Name;
            donationItem.Description = donationItemDTO.Description;
            donationItem.Price = donationItemDTO.Price;

            _unitOfWork.DonationItems.Update(donationItem);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("Status: NoContent. Donation item was edited.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Bad request. Id must be greater than zero.");
                return BadRequest();
            }

            var donationItem = await _unitOfWork.DonationItems.GetById(id);

            if (donationItem == null)
            {
                _logger.LogError("Bad request. No donation item with such id was found");
                return BadRequest();
            }

            donationItem.IsDeleted = true;

            _unitOfWork.DonationItems.Update(donationItem);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("Status: OK. Donation item was deleted.");

            return Ok();
        }
    }
}