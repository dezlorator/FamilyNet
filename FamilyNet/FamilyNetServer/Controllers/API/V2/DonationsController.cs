﻿using System;
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
using Microsoft.AspNetCore.Authorization;

namespace FamilyNetServer.Controllers.API.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DonationDTO> _donationValidator;
        private readonly IDonationsFilter _donationsFilter;
        private readonly ILogger<DonationsController> _logger;
        
        #endregion

        #region ctor

        public DonationsController(IUnitOfWork unitOfWork,
                                   IValidator<DonationDTO> donationValidator,
                                   IDonationsFilter donationsFilter,
                                   ILogger<DonationsController> logger)
        {
            _unitOfWork = unitOfWork;
            _donationValidator = donationValidator;
            _donationsFilter = donationsFilter;
            _logger = logger;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]int rows,
                                    [FromQuery]int page,
                                    [FromQuery]string forSearch,
                                    [FromQuery]string status = "Needed")
        {
            var donations = _unitOfWork.Donations.GetAll().Where(c => !c.IsDeleted);

            if (!Enum.TryParse(status, out DonationStatus donationStatus))
            {
                return BadRequest();
            }

            donations = _donationsFilter.GetDonations(donations, forSearch, donationStatus);

            if (rows != 0 && page != 0)
            {
                _logger.LogInformation("Paging were used");
                donations = donations
                    .Skip((page - 1) * rows).Take(rows);
            }

            if (donations == null)
            {
                _logger.LogError("Bad request. No donations were found");
                return BadRequest();
            }

            var donationsDTO = new List<DonationDTO>();

            donationsDTO = donations.Select(d =>
                new DonationDTO()
                {
                    ID = d.ID,
                    City = d.Orphanage.Adress.City,
                    OrphanageName = d.Orphanage.Name,
                    DonationItemID = d.DonationItemID,
                    OrphanageID = d.OrphanageID,
                    CharityMakerID = d.CharityMakerID,
                    Status = d.Status.ToString(),
                    LastDateWhenStatusChanged = d.LastDateWhenStatusChanged,
                    ItemName = d.DonationItem.Name,
                    ItemDescription = d.DonationItem.Description,
                    Types = d.DonationItem.TypeBaseItem.Select(t => new CategoryDTO
                    {
                        Name = t.Type.Name,
                        ID = t.TypeID
                    })
                }).ToList();

            _logger.LogInformation("Status: OK. List of donations was sent");
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
                _logger.LogError("Bad request. No donation was found");
                return BadRequest();
            }

            var donationDetailsDTO = new DonationDetailDTO()
            {
                ID = donation.ID,
                Types = donation.DonationItem.TypeBaseItem.Select(t => new CategoryDTO
                {
                    Name = t.Type.Name,
                    ID = t.TypeID
                }),
                DonationItemID = donation.DonationItemID,
                CharityMakerID = donation.CharityMakerID,
                OrphanageID = donation.OrphanageID,
                ItemName = donation.DonationItem.Name,
                ItemDescription = donation.DonationItem.Description,
                OrphanageName = donation.Orphanage.Name,
                City = donation.Orphanage.Adress.City,
                House = donation.Orphanage.Adress.House,
                Street = donation.Orphanage.Adress.Street,
                Rating = donation.Orphanage.Rating,
                Status = donation.Status.ToString(),
                LastDateWhenStatusChanged = donation.LastDateWhenStatusChanged
            };

            _logger.LogInformation("Status: OK. Donation was sent");
            return Ok(donationDetailsDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker, Representative")]
        public async Task<IActionResult> Create([FromBody]DonationDTO donationDTO)
        {
            if (!_donationValidator.IsValid(donationDTO))
            {
                _logger.LogError("Model is not valid.");
                return BadRequest();
            }

            var donation = new Donation()
            {
                DonationItemID = donationDTO.DonationItemID,
                DonationItem = await _unitOfWork.DonationItems.GetById(donationDTO.DonationItemID.Value),
                CharityMakerID = donationDTO.CharityMakerID,
                OrphanageID = donationDTO.OrphanageID,
                Orphanage = await _unitOfWork.Orphanages.GetById(donationDTO.OrphanageID.Value),
                Status = DonationStatus.Needed,
                LastDateWhenStatusChanged = DateTime.Now
            };

            await _unitOfWork.Donations.Create(donation);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("Status: Created. Donation was created");
            return Created("api/v1/donations/" + donation.ID, donationDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin, CharityMaker, Representative, Volunteer")]
        public async Task<IActionResult> Edit(int id, [FromBody]DonationDTO donationDTO)
        {
            if (!_donationValidator.IsValid(donationDTO))
            {
                _logger.LogError("Model is not valid.");
                return BadRequest();
            }

            var donation = await _unitOfWork.Donations.GetById(id);

            if (donation == null)
            {
                _logger.LogError("Bad request. No donation was found");
                return BadRequest();
            }

            if (donationDTO.Status == null)
            {
                donationDTO.Status = "Needed";
            }

            if (!Enum.TryParse(donationDTO.Status, out DonationStatus status))
            {
                return BadRequest();
            }

            if (status != donation.Status)
            {
                donation.LastDateWhenStatusChanged = DateTime.Now;
            }

            donation.Status = status;

            if (donation.DonationItemID != null)
            {
                _logger.LogInformation("Donation item is not null.");
                donation.DonationItemID = donationDTO.DonationItemID;
                donation.DonationItem = await _unitOfWork.DonationItems.GetById(donation.DonationItemID.Value);
            }

            if (donationDTO.CharityMakerID != donation.CharityMakerID)
            {
                donation.Status = DonationStatus.Sent;
            }

            if (donationDTO.OrphanageID != null)
            {
                _logger.LogInformation("Orphanage is not null.");
                donation.OrphanageID = donationDTO.OrphanageID;
                donation.Orphanage = await _unitOfWork.Orphanages.GetById(donation.OrphanageID.Value);
            }

            _unitOfWork.Donations.Update(donation);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("Status: NoContent. Donation was edited.");

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

            var donation = await _unitOfWork.Donations.GetById(id);

            if (donation == null)
            {
                _logger.LogError("Bad request. No donation with such id was found");
                return BadRequest();
            }

            donation.IsDeleted = true;

            _unitOfWork.Donations.Update(donation);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("Status: OK. Donation was deleted.");

            return Ok();
        }
    }
}