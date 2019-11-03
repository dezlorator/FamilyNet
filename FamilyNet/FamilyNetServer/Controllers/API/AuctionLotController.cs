﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;
using FamilyNetServer.Configuration;
using FamilyNetServer.Enums;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuctionLotController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _repository;
        private readonly IFileUploader _fileUploader;
        private readonly IValidator<AuctionLotDTO> _auctionValidator;
        private readonly ILogger<AuctionLotController> _logger;
        private readonly IOptionsSnapshot<ServerURLSettings> _settings;

        #endregion

        #region ctor

        public AuctionLotController(IUnitOfWork repo, 
            IFileUploader fileUploader,
            IValidator<AuctionLotDTO> auctionValidator,
            ILogger<AuctionLotController> logger,
            IOptionsSnapshot<ServerURLSettings> settings)
        {
            _repository = repo;
            _fileUploader = fileUploader;
            _auctionValidator = auctionValidator;
            _logger = logger;
            _settings = settings;
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll()
        {
            var auction = _repository.AuctionLots.GetAll().Where(c => !c.IsDeleted);

            if (auction == null)
            {
                return BadRequest();
            }

            var auctions = new List<AuctionLotDTO>();

            foreach (var lot in auction)
            {
                var auctionLotDTO = new AuctionLotDTO()
                {
                    ID = lot.ID,
                    DateStart = lot.DateStart,
                    DateEnd = lot.DateEnd,
                    OrphanID = lot.OrphanID,
                    Quantity = lot.Quantity,
                    Status = lot.Status.ToString(),
                    PhotoParth = _settings.Value.ServerURL + lot.Avatar,
                    AuctionLotItemID = lot.AuctionLotItemID,
                };

                auctions.Add(auctionLotDTO);
            }

            _logger.LogInformation("Returned auction lots list");
            return Ok(auctions);
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var auction = await _repository.AuctionLots.GetById(id);

            if (auction == null)
            {
                _logger.LogError($"No auction lot with id #{id} in database");
                return BadRequest();
            }

            var auctionDTO = new AuctionLotDTO()
            {
                ID = auction.ID,
                DateStart = auction.DateStart,
                DateEnd = auction.DateEnd,
                OrphanID = auction.OrphanID,
                Quantity = auction.Quantity,
                Status = auction.Status.ToString(),
                PhotoParth = _settings.Value.ServerURL + auction.Avatar,
                AuctionLotItemID = auction.AuctionLotItemID,
            };

            _logger.LogInformation($"Returned auction lot #{id}");
            return Ok(auctionDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Orphan")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]AuctionLotDTO auctionDTO)
        {
            if (!_auctionValidator.IsValid(auctionDTO))
            {
                _logger.LogError("Invalid auction lot");
                return BadRequest();
            }

            var auction = new AuctionLot()
            {
                DateStart = auctionDTO.DateStart,
                DateEnd = auctionDTO.DateEnd,
                OrphanID = auctionDTO.OrphanID,
                Quantity = auctionDTO.Quantity,
                Status =  AuctionLotStatus.UnApproved,
                AuctionLotItemID = auctionDTO.AuctionLotItemID,
                IsDeleted = false
            };

            var pathPhoto = String.Empty;

            if (auctionDTO.Avatar != null)
            {
                var fileName = auctionDTO.ID.ToString() + DateTime.Now.Ticks;

                pathPhoto = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.Auction), auctionDTO.Avatar);
            }

            auction.Avatar = pathPhoto;


            await _repository.AuctionLots.Create(auction);
            _repository.SaveChangesAsync();

            auctionDTO.ID = auction.ID;

            _logger.LogInformation($"Created auction lot with id #{auction.ID}");

            return Created(auctionDTO.ID.ToString(), auctionDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Orphan, Representative")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromForm]AuctionLotDTO auctionDTO)
        {
            if (!_auctionValidator.IsValid(auctionDTO))
            {
                _logger.LogError("Invalid auction lot");
                return BadRequest();
            }

            var auction = await _repository.AuctionLots.GetById(id);

            if (auction == null)
            {
                return BadRequest();
            }

            auction.AuctionLotItemID = auctionDTO.AuctionLotItemID;
            auction.DateEnd = auctionDTO.DateEnd;
            auction.DateStart = auctionDTO.DateStart;
            auction.OrphanID = auctionDTO.OrphanID;
            var status = AuctionLotStatus.UnApproved;
            Enum.TryParse(auctionDTO.Status,out status);
            auction.Status = status;
            auction.Quantity = auctionDTO.Quantity;

            if (auctionDTO.Avatar != null)
            {
                var fileName = auctionDTO.ID.ToString() + DateTime.Now.Ticks;

                auction.Avatar = _fileUploader.CopyFileToServer(fileName,
                        nameof(DirectoryUploadName.Auction), auctionDTO.Avatar);
            }

            _repository.AuctionLots.Update(auction);
            _repository.SaveChangesAsync();

            _logger.LogInformation($"Edited auction lot with id #{auction.ID}");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Orphan")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"No auction lot with id #{id} in database");
                return BadRequest();
            }

            var auction = await _repository.AuctionLots.GetById(id);

            if (auction == null)
            {
                _logger.LogError($"No auction lot with id #{id} in database");
                return BadRequest();
            }

            auction.IsDeleted = true;

            _repository.AuctionLots.Update(auction);
            _repository.SaveChangesAsync();

            _logger.LogInformation($"Deleted auction lot with id #{auction.ID}");

            return Ok();
        }
    }
}