using DataTransferObjects;
using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DonationItemsController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWork _unitOfWork;
         private readonly IDonationItemValidator _donationItemValidator;
         private readonly IDonationItemsFilter _donationItemsFilter;

        #endregion

        public DonationItemsController(IUnitOfWork unitOfWork,
                                  IDonationItemValidator donationItemValidator,
                                  IDonationItemsFilter donationItemsFilter)
        {
            _unitOfWork = unitOfWork;
            _donationItemValidator = donationItemValidator;
            _donationItemsFilter = donationItemsFilter;
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
                donationItems = donationItems.
                    Skip((page - 1) * rows).Take(rows);
            }

            if (donationItems == null)
            {
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

            return Ok(donationItemDTO);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]DonationItemDTO donationItemDTO)
        {
            if (!_donationItemValidator.IsValid(donationItemDTO))
            {
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
            }

            await _unitOfWork.DonationItems.Create(donationItem);
            _unitOfWork.SaveChangesAsync();

            donationItemDTO.ID = donationItem.ID;

            return Created("api/v1/donationItems/" + donationItem.ID, donationItemDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]DonationItemDTO donationItemDTO)
        {
            if (!_donationItemValidator.IsValid(donationItemDTO))
            {
                return BadRequest();
            }

            var donationItem = await _unitOfWork.DonationItems.GetById(id);

            if (donationItem == null)
            {
                return BadRequest();
            }

            donationItem.Name = donationItemDTO.Name;
            donationItem.Description = donationItemDTO.Description;
            donationItem.Price = donationItemDTO.Price;

            _unitOfWork.DonationItems.Update(donationItem);
            _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var donationItem = await _unitOfWork.DonationItems.GetById(id);

            if (donationItem == null)
            {
                return BadRequest();
            }

            donationItem.IsDeleted = true;

            _unitOfWork.DonationItems.Update(donationItem);
            _unitOfWork.SaveChangesAsync();

            return Ok();
        }
    }
}