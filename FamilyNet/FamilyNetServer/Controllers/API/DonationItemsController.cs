using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.Models;
using FamilyNetServer.Filters;
using FamilyNetServer.Validators;
using DataTransferObjects;
using Microsoft.Extensions.Logging;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DonationItemsController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDonationItemValidator _donationItemValidator;
        private readonly IDonationItemsFilter _donationItemsFilter;
        private readonly ILogger<DonationItem> _logger;

        #endregion

        public DonationItemsController(IUnitOfWorkAsync unitOfWork,
                                  IDonationItemValidator donationItemValidator,
                                  IDonationItemsFilter donationItemsFilter,
                                  ILogger<DonationItem> logger)
        {
            _unitOfWork = unitOfWork;
            _donationItemValidator = donationItemValidator;
            _donationItemsFilter = donationItemsFilter;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]int rows,
                                          [FromQuery]int page,
                                          [FromQuery]string name,
                                          [FromQuery]float minPrice,
                                          [FromQuery]float maxPrice,
                                          [FromQuery]string category
                                   )
        {
            var donationItems = _unitOfWork.DonationItems.GetAll().Where(b => !b.IsDeleted);
            donationItems = _donationItemsFilter.GetDonationItems(donationItems, name, minPrice, maxPrice, category);

            if (rows != 0 && page != 0)
            {
                _logger.LogInformation("Paging were used");
                donationItems = donationItems.
                    Skip((page - 1) * rows).Take(rows);
            }

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

            _logger.LogInformation("List of donation items was sent");

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

            _logger.LogInformation("Donation item was sent");
            return Ok(donationItemDTO);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]DonationItemDTO donationItemDTO)
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
                foreach (int c in donationItemDTO.CategoriesID)
                {
                    var itemType = new TypeBaseItem
                    {
                        ItemID = ID,
                        TypeID = c
                    };

                    donationItem.TypeBaseItem.Add(itemType);
                }

                _logger.LogInformation("Categories were added.");
            }

            await _unitOfWork.DonationItems.Create(donationItem);
            _unitOfWork.SaveChangesAsync();

            donationItemDTO.ID = donationItem.ID;

            _logger.LogInformation("Donation was created");

            return Created("api/v1/donationItems/" + donationItem.ID, donationItemDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]DonationItemDTO donationItemDTO)
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
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Donation item was edited.");

            return NoContent();
        }

        [HttpDelete("{id}")]
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
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Donation item was deleted.");

            return Ok();
        }
    }
}