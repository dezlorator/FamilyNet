using DataTransferObjects;
using FamilyNetServer.Enums;
using FamilyNetServer.Uploaders;
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
    public class DonationsController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDonationValidator _donationValidator;
        private readonly IDonationsFilter _donationsFilter;

        #endregion

        public DonationsController(IUnitOfWorkAsync unitOfWork,
                                   IDonationValidator donationValidator,
                                   IDonationsFilter donationsFilter)
        {
            _unitOfWork = unitOfWork;
            _donationValidator = donationValidator;
            _donationsFilter = donationsFilter;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromForm]int rows,
                                    [FromForm]int page,
                                    [FromForm]int? orphanageID)
        {
            var donations = _unitOfWork.Donations.GetAll().Where(c => !c.IsDeleted);

            if (rows != 0 && page != 0)
            {
                donations = _donationsFilter.GetDonations(donations, orphanageID)
                    .Skip((page - 1) * rows).Take(rows);
            }

            if (donations == null)
            {
                return BadRequest();
            }

            var donationsDTO = new List<DonationDTO>();

            foreach (var d in donations)
            {
                donationsDTO.Add(new DonationDTO
                {
                    ID = d.ID,
                    City = d.Orphanage.Adress.City,
                    PathToAvatar = d.Orphanage.Avatar,
                    Status = d.Status.ToString(),
                    LastDateWhenStatusChanged = d.LastDateWhenStatusChanged,
                    ItemName = d.DonationItem.Name,
                    ItemDescription = d.DonationItem.Description,
                    Types = d.DonationItem.TypeBaseItem
                             .Select(t => t.TypeID)
                }
                );
            }

            return Ok(donationsDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var donation = await _unitOfWork.Donations.GetById(id);

            if (donation == null)
            {
                return BadRequest();
            }

            var donationDetailsDTO = new DonationDetailDTO()
            {
                Categories = donation.DonationItem.TypeBaseItem
                                     .Select(t => t.Type.Name),
                ItemName = donation.DonationItem.Name,
                ItemDescription = donation.DonationItem.Description,
                OrphanageName = donation.Orphanage.Name,
                OrphanageCity = donation.Orphanage.Adress.City,
                OrphanageHouse = donation.Orphanage.Adress.House,
                OrphanageStreet = donation.Orphanage.Adress.Street,
                OrphanageRating = donation.Orphanage.Rating
            };

            return Ok(donationDetailsDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]DonationDTO donationDTO)
        {
            if (!_donationValidator.IsValid(donationDTO))
            {
                return BadRequest();
            }

            var donation = new Donation()
            {
                DonationItemID = donationDTO.DonationItemID,
                CharityMakerID = donationDTO.CharityMakerID,
                OrphanageID = donationDTO.OrphanageID,
                Status = DonationStatus.Sended,
                LastDateWhenStatusChanged = DateTime.Now
            };

            await _unitOfWork.Donations.Create(donation);
            _unitOfWork.SaveChangesAsync();

            return Created("api/v1/donations/" + donation.ID, donationDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]DonationDTO donationDTO)
        {
            if (!_donationValidator.IsValid(donationDTO))
            {
                return BadRequest();
            }

            var donation = await _unitOfWork.Donations.GetById(id);

            if (donation == null)
            {
                return BadRequest();
            }

            donation.DonationItemID = donationDTO.DonationItemID;
            donation.IsRequest = true;
            donation.CharityMakerID = donationDTO.CharityMakerID;
            donation.OrphanageID = donationDTO.OrphanageID;

            _unitOfWork.Donations.Update(donation);
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

            var donation = await _unitOfWork.Donations.GetById(id);

            if(donation == null)
            {
                return BadRequest();
            }

            donation.IsDeleted = true;

            _unitOfWork.Donations.Update(donation);
            _unitOfWork.SaveChangesAsync();

            return Ok();
        }
    }
}