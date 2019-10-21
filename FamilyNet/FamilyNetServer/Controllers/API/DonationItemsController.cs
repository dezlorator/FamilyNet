using FamilyNetServer.DTO;
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

        private readonly IUnitOfWorkAsync _unitOfWork;
         private readonly IDonationItemValidator _donationItemValidator;
         private readonly IDonationItemsFilter _donationItemsFilter;

        #endregion

        public DonationItemsController(IUnitOfWorkAsync unitOfWork,
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
        public async Task<IActionResult> GetAll([FromQuery]int rows,
                                          [FromQuery]int page,
                                          [FromQuery]string name,
                                          [FromQuery]float minPrice,
                                          [FromQuery]float maxPrice,
                                          [FromQuery]string category
                                   )
        {
            var donationItems = _unitOfWork.DonationItems.GetAll().Where(b => !b.IsDeleted);


            if (rows != 0 && page != 0)
            {
                donationItems = _donationItemsFilter.GetDonationItems(donationItems, name, minPrice, maxPrice, category)
                    .Skip((page - 1) * rows).Take(rows);
            }

            if (donationItems == null)
            {
                return BadRequest();
            }

            var donationItemsDTO = new List<DonationItemDTO>();

            foreach (var d in donationItems)
            {
                var item = new DonationItemDTO
                {
                    Name = d.Name,
                    Description = d.Description,
                    Price = d.Price,
                    CategoriesID = d.TypeBaseItem.Select(t => t.TypeID)
                };

                item.Categories = new List<string>();

                foreach (TypeBaseItem t in d.TypeBaseItem)
                {
                    var type = await _unitOfWork.BaseItemTypes.GetById(t.TypeID);

                    if (type != null)
                    {
                        item.Categories.Add(type.Name);
                    }
                }

                donationItemsDTO.Add(item);
            }

            return Ok(donationItemsDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]DonationItemDTO donationItemDTO)
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

            foreach (int c in donationItemDTO.CategoriesID)
            {
                var itemType = new TypeBaseItem
                {
                    ItemID = ID,
                    TypeID = c
                };

                donationItem.TypeBaseItem.Add(itemType);
            }

            await _unitOfWork.DonationItems.Create(donationItem);
            _unitOfWork.SaveChangesAsync();

            return Created("api/v1/donations/" + donationItem.ID, donationItemDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromBody]DonationItemDTO donationItemDTO)
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