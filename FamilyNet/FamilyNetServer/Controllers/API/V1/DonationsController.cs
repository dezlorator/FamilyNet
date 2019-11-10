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
using FamilyNetServer.HttpHandlers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDonationValidator _donationValidator;
        private readonly IDonationsFilter _donationsFilter;
        private readonly ILogger<DonationsController> _logger;
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public DonationsController(IUnitOfWork unitOfWork,
                                   IDonationValidator donationValidator,
                                   IDonationsFilter donationsFilter,
                                   ILogger<DonationsController> logger,
                                   IIdentityExtractor identityExtractor)
        {
            _unitOfWork = unitOfWork;
            _donationValidator = donationValidator;
            _donationsFilter = donationsFilter;
            _logger = logger;
            _identityExtractor = identityExtractor;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery]int rows,
                                    [FromQuery]int page,
                                    [FromQuery]string forSearch)
        {
            _logger.LogInformation("{info}",
                   "Endpoint Donations/api/v1 GetAll was called");

            var donations = _unitOfWork.Donations.GetAll().Where(c => !c.IsDeleted);
            donations = _donationsFilter.GetDonations(donations, forSearch);

            if (rows > 0 && page > 0)
            {
                _logger.LogInformation("{info}", "Paging were used");
                donations = donations.Skip((page - 1) * rows).Take(rows);
            }

            if (donations == null)
            {
                _logger.LogInformation("{status}{info}",
                                   StatusCodes.Status400BadRequest,
                                   "List of Donations is empty");

                return BadRequest();
            }

            var donationsDTO = new List<DonationDTO>();

            donationsDTO = await donations.Select(d =>
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
                    Types = d.DonationItem.TypeBaseItem
                               .Select(t => t.TypeID)
                }).ToListAsync();

            _logger.LogInformation("{status}, {json}",
              StatusCodes.Status200OK,
              JsonConvert.SerializeObject(donationsDTO));

            return Ok(donationsDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                  $"Endpoint Donations/api/v1 GetById({id}) was called");

            var donation = await _unitOfWork.Donations.GetById(id);

            if (donation == null)
            {
                _logger.LogError("{info}{status}", $"Donation wasn't found [id:{id}]",
                                   StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var donationDetailsDTO = new DonationDetailDTO()
            {
                ID = donation.ID,
                Types = donation.DonationItem.TypeBaseItem
                                     .Select(t => t.TypeID),
                DonationItemID = donation.DonationItemID,
                CharityMakerID = donation.CharityMakerID,
                OrphanageID = donation.OrphanageID,
                ItemName = donation.DonationItem.Name,
                ItemDescription = donation.DonationItem.Description,
                OrphanageName = donation.Orphanage.Name,
                City = donation.Orphanage.Adress.City,
                OrphanageHouse = donation.Orphanage.Adress.House,
                OrphanageStreet = donation.Orphanage.Adress.Street,
                OrphanageRating = donation.Orphanage.Rating
            };

            _logger.LogInformation("{status},{json}",
                           StatusCodes.Status200OK,
                           JsonConvert.SerializeObject(donationDetailsDTO));

            return Ok(donationDetailsDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin, Volunteer, CharityMaker, Representative")]
        public async Task<IActionResult> Create([FromForm]DonationDTO donationDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Donations/api/v1 [POST] was called", userId, token);

            if (!_donationValidator.IsValid(donationDTO))
            {
                _logger.LogWarning("{status}{token}{userId}",
                    StatusCodes.Status400BadRequest,
                    token, userId);

                return BadRequest();
            }

            var donation = new Donation()
            {
                DonationItemID = donationDTO.DonationItemID,
                DonationItem = await _unitOfWork.DonationItems.GetById(donationDTO.DonationItemID.Value),
                CharityMakerID = donationDTO.CharityMakerID,
                OrphanageID = donationDTO.OrphanageID,
                Orphanage = await _unitOfWork.Orphanages.GetById(donationDTO.OrphanageID.Value),
                Status = DonationStatus.Sended,
                LastDateWhenStatusChanged = DateTime.Now
            };

            await _unitOfWork.Donations.Create(donation);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Donation was saved [id:{donation.ID}]");

            return Created("api/v1/donations/" + donation.ID, donationDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [FromForm]DonationDTO donationDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Donations/api/v1 [PUT] was called", userId, token);

            if (!_donationValidator.IsValid(donationDTO))
            {
                _logger.LogError("{userId} {token} {status} {info}", userId, token,
                                    StatusCodes.Status400BadRequest.ToString(),
                                    "Donation enity is invalid");

                return BadRequest();
            }

            var donation = await _unitOfWork.Donations.GetById(id);

            if (donation == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                                   StatusCodes.Status400BadRequest,
                                   $"Donation was not found [id:{id}]",
                                   userId, token);

                return BadRequest();
            }

            if (donation.DonationItemID != null)
            {
                _logger.LogInformation("{status} {info} {userId} {token}",
                                  StatusCodes.Status400BadRequest,
                                  "Donation item is not null.",
                                  userId, token);

                donation.DonationItemID = donationDTO.DonationItemID;
                donation.DonationItem = await _unitOfWork.DonationItems.GetById(donation.DonationItemID.Value);
            }

            donation.IsRequest = true;

            donation.CharityMakerID = donationDTO.CharityMakerID;

            if (donationDTO.OrphanageID != null)
            {
                _logger.LogInformation("{status} {info} {userId} {token}",
                                  StatusCodes.Status400BadRequest,
                                  "Orphanage is not null.",
                                  userId, token);

                donation.OrphanageID = donationDTO.OrphanageID;
                donation.Orphanage = await _unitOfWork.Orphanages.GetById(donation.OrphanageID.Value);
            }

            _unitOfWork.Donations.Update(donation);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{token}{userId}{status}{info}",
                 token, userId, StatusCodes.Status204NoContent,
                 $"Donation was updated [id:{donation.ID}]");

            return NoContent();
        }

        [HttpPut("StatusEdit/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StatusEdit(int id, [FromForm]string status)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Children/api/v1/StatusEdit/ [PUT] was called", userId, token);

            if (!Enum.TryParse(status, out DonationStatus donationStatus))
            {
                _logger.LogError("Wrong string.");
                return BadRequest();
            }

            var donation = await _unitOfWork.Donations.GetById(id);

            if (donation == null)
            {
                _logger.LogError("Bad request. No donation with such id was found");
                return BadRequest();
            }

            donation.Status = donationStatus;

            _unitOfWork.Donations.Update(donation);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Status: NoContent. Donation status was edited.");

            return NoContent();
        }

        [HttpPut("donationMade/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCharityMaker(int id, [FromForm]int charityMakerID)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Donations/api/v1/donationMade/ [PUT] was called", userId, token);

            CharityMaker charityMaker = await _unitOfWork.CharityMakers.GetById(charityMakerID);

            if (charityMaker == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest, $"Charity maker was not found [id:{id}]",
                    userId, token);

                return BadRequest();
            }

            Donation donation = await _unitOfWork.Donations.GetById(id);

            if (donation == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest, $"Donation was not found [id:{id}]",
                    userId, token);

                return BadRequest();
            }

            donation.CharityMakerID = charityMakerID;

            _unitOfWork.Donations.Update(donation);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status204NoContent,
                $"Charity maker was added [id:{charityMakerID}]");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
               "Endpoint Donations/api/v1 [DELETE] was called", userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                                   StatusCodes.Status400BadRequest,
                                   $"Argument id is not valid [id:{id}]",
                                   userId, token);

                return BadRequest();
            }

            var donation = await _unitOfWork.Donations.GetById(id);

            if (donation == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                StatusCodes.Status400BadRequest,
                $"Donation was not found [id:{id}]",
                userId, token);

                return BadRequest();
            }

            donation.IsDeleted = true;

            _unitOfWork.Donations.Update(donation);
            _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{status} {info} {userId} {token}",
                           StatusCodes.Status200OK,
                           $"Donation.IsDelete was updated [id:{id}]",
                           userId, token);
            return Ok();
        }
    }
}