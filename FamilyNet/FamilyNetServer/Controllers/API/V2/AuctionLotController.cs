 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;
using FamilyNetServer.Configuration;
using FamilyNetServer.Enums;
using FamilyNetServer.HttpHandlers;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FamilyNetServer.Controllers.API.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class AuctionLotController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _repository;
        private readonly IFileUploader _fileUploader;
        private readonly IValidator<AuctionLotDTO> _auctionValidator;
        private readonly ILogger<AuctionLotController> _logger;
        private readonly IOptionsSnapshot<ServerURLSettings> _settings;
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public AuctionLotController(IUnitOfWork repo,
            IFileUploader fileUploader,
            IValidator<AuctionLotDTO> auctionValidator,
            ILogger<AuctionLotController> logger,
            IOptionsSnapshot<ServerURLSettings> settings,
            IIdentityExtractor identityExtractor)
        {
            _repository = repo;
            _fileUploader = fileUploader;
            _auctionValidator = auctionValidator;
            _logger = logger;
            _settings = settings;
            _identityExtractor = identityExtractor;
        }

        #endregion


        [HttpGet("approved")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllApproved([FromQuery]string name,
                                   [FromQuery]string priceStart,
                                   [FromQuery]string priceEnd,
                                    [FromQuery]string sort,
                                   [FromQuery]int rows,
                                   [FromQuery]int page)
        {
            _logger.LogInformation("{info}",
                "Endpoint AuctionLot/api/v1 GetAllApproved was called");

            float.TryParse(priceStart, out var start);
            float.TryParse(priceEnd, out var end);
            Enum.TryParse<AuctionLotSortState>(sort, out var sortOrder);

            var auction = _repository.AuctionLots.GetAll().Where(c => !c.IsDeleted
            && c.Status == AuctionLotStatus.Approved && c.Quantity > 0);

            foreach (var lot in auction)
            {
                lot.AuctionLotItem = await GetItem(lot.AuctionLotItemID.Value);
            }

            var crafts = GetFiltered(auction, start, end, name, page, rows, sortOrder, out int count);

            if (crafts == null)
            {
                _logger.LogWarning("{status}{info}",
                   StatusCodes.Status400BadRequest,
                   "List of Auction Lots is empty");

                return BadRequest();
            }

            var auctions = crafts.Select(lot =>
                new AuctionLotDTO()
                {
                    ID = lot.ID,
                    DateAdded = lot.DateAdded,
                    OrphanID = lot.OrphanID,
                    Quantity = lot.Quantity,
                    Status = lot.Status.ToString(),
                    PhotoParth = _settings.Value.ServerURL + lot.Avatar,
                    AuctionLotItemID = lot.AuctionLotItemID,
                }).ToList();


            var filterModel = new AuctionLotFilterDTO
            {
                AuctionLotDTOs = auctions,
                TotalCount = count
            };


            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(auctions));

            return Ok(filterModel);
        }


        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAllOrphanCrafts([FromQuery]int orphanId, [FromQuery]int rows,
                           [FromQuery]int page)
        {
            _logger.LogInformation("{info}",
                "Endpoint AuctionLot/api/v1 GetAllOrphanCrafts was called");

            var auction = _repository.AuctionLots.GetAll().Where(c => !c.IsDeleted
            && c.Status != AuctionLotStatus.Approved && c.OrphanID == orphanId);

            if (page > 0 && rows > 0)
            {
                auction = auction.Skip((page - 1) * rows).Take(rows);
            }

            if (auction == null)
            {
                _logger.LogWarning("{status}{info}",
                   StatusCodes.Status400BadRequest,
                   "List of Auction Lots is empty");

                return BadRequest();
            }

            var auctions = auction.Select(lot =>
               new AuctionLotDTO()
               {
                   ID = lot.ID,
                   DateAdded = lot.DateAdded,
                   OrphanID = lot.OrphanID,
                   Quantity = lot.Quantity,
                   Status = lot.Status.ToString(),
                   PhotoParth = _settings.Value.ServerURL + lot.Avatar,
                   AuctionLotItemID = lot.AuctionLotItemID,
               }).ToList();

            var filterModel = new AuctionLotFilterDTO
            {
                AuctionLotDTOs = auctions,
                TotalCount = auction.Count()
            };


            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
               JsonConvert.SerializeObject(auctions));

            return Ok(filterModel);
        }

        [HttpGet("confirm")]
        [Authorize(Roles = "Orphan, Representative")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAllUnApproved([FromQuery]int orphanId)
        {
            _logger.LogInformation("{info}",
                "Endpoint AuctionLot/api/v1 GetAllUnApproved was called");

            var auction = _repository.AuctionLots.GetAll().Where(c => !c.IsDeleted
             && c.Status == AuctionLotStatus.UnApproved && c.OrphanID == orphanId);

            if (auction == null)
            {
                _logger.LogWarning("{status}{info}",
                   StatusCodes.Status400BadRequest,
                   "List of Auction Lots is empty");

                return BadRequest();
            }

            var auctions = auction.Select(lot =>
               new AuctionLotDTO()
               {
                   ID = lot.ID,
                   DateAdded = lot.DateAdded,
                   OrphanID = lot.OrphanID,
                   Quantity = lot.Quantity,
                   Status = lot.Status.ToString(),
                   PhotoParth = _settings.Value.ServerURL + lot.Avatar,
                   AuctionLotItemID = lot.AuctionLotItemID,
               }).ToList();

            var filterModel = new AuctionLotFilterDTO
            {
                AuctionLotDTOs = auctions,
                TotalCount = auction.Count()
            };

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
               JsonConvert.SerializeObject(auctions));

            return Ok(filterModel);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Orphan, Representative")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                $"Endpoint AuctionLot/api/v1 GetById({id}) was called");

            var auction = await _repository.AuctionLots.GetById(id);

            if (auction == null)
            {
                _logger.LogError("{info}{status}",
                    $"Auction Lot wasn't found. [id:{id}]",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var auctionDTO = new AuctionLotDTO()
            {
                ID = auction.ID,
                DateAdded = auction.DateAdded,
                OrphanID = auction.OrphanID,
                Quantity = auction.Quantity,
                Status = auction.Status.ToString(),
                PhotoParth = _settings.Value.ServerURL + auction.Avatar,
                AuctionLotItemID = auction.AuctionLotItemID,
            };

            _logger.LogInformation("{status},{json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(auctionDTO));

            return Ok(auctionDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Orphan, Representative")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]AuctionLotDTO auctionDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint AuctionLot/api/v1 [POST] was called", userId, token);

            if (!_auctionValidator.IsValid(auctionDTO))
            {
                _logger.LogWarning("{status}{token}{userId}",
                    StatusCodes.Status400BadRequest, token, userId);

                return BadRequest();
            }

            var auction = new AuctionLot()
            {
                DateAdded = DateTime.Now,
                OrphanID = auctionDTO.OrphanID,
                Quantity = auctionDTO.Quantity,
                Status = AuctionLotStatus.UnApproved,
                AuctionLotItemID = auctionDTO.AuctionLotItemID,
                IsDeleted = false
            };

            var pathPhoto = String.Empty;

            if (auctionDTO.Avatar != null)
            {
                _logger.LogInformation("{info}", "AuctionLotDTO has file photo.");

                var fileName = auctionDTO.ID.ToString() + DateTime.Now.Ticks;

                pathPhoto = _fileUploader.CopyFileToServer(fileName,
                    nameof(DirectoryUploadName.Auction), auctionDTO.Avatar);
            }

            auction.Avatar = pathPhoto;

            await _repository.AuctionLots.Create(auction);
            _repository.SaveChanges();

            auctionDTO.ID = auction.ID;

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Auction Lot was saved [id:{auction.ID}]");

            return Created(auctionDTO.ID.ToString(), auctionDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Orphan, Representative")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]AuctionLotDTO auctionDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint AuctionLot/api/v1 [PUT] was called", userId, token);

            if (!_auctionValidator.IsValid(auctionDTO))
            {
                _logger.LogError("{userId} {token} {status} {info}", userId,
                    token, StatusCodes.Status400BadRequest.ToString(),
                    "AuctionLotDTO is invalid");

                return BadRequest();
            }

            var auction = await _repository.AuctionLots.GetById(id);

            if (auction == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Auction Lot was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            auction.AuctionLotItemID = auctionDTO.AuctionLotItemID;
            auction.DateAdded = auctionDTO.DateAdded;
            auction.OrphanID = auctionDTO.OrphanID;
            var status = AuctionLotStatus.UnApproved;
            Enum.TryParse(auctionDTO.Status, out status);
            auction.Status = status;
            auction.Quantity = auctionDTO.Quantity;

            if (auctionDTO.Avatar != null)
            {
                _logger.LogInformation("{info}", "AuctionLotDTO has file photo.");

                var fileName = auctionDTO.ID.ToString() + DateTime.Now.Ticks;

                auction.Avatar = _fileUploader.CopyFileToServer(fileName,
                    nameof(DirectoryUploadName.Auction), auctionDTO.Avatar);
            }

            _repository.AuctionLots.Update(auction);
            _repository.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status204NoContent,
                $"Auction Lot was updated [id:{auction.ID}]");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Orphan")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint AuctionLots/api/v1 [DELETE] was called",
                userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            var auction = await _repository.AuctionLots.GetById(id);

            if (auction == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"AuctionLot was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            auction.IsDeleted = true;

            _repository.AuctionLots.Update(auction);
            _repository.SaveChanges();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"AuctionLot.IsDelete was updated [id:{id}]", userId, token);

            return Ok();
        }

        #region Private Helpers
        private async Task<AuctionLotItem> GetItem(int id)
        {
            var item = await _repository.DonationItems.GetById(id);

            return new AuctionLotItem
            {
                ID = item.ID,
                Name = item.Name,
                Price = item.Price,
                Description = item.Description
            };
        }

        private IEnumerable<AuctionLot> GetFiltered(IEnumerable<AuctionLot> lots, float start, float end, string name, int page,
            int rows, AuctionLotSortState sortOrder, out int count)
        {

            if (start > 0.0)
            {
                lots = lots.Where(o => o.AuctionLotItem.Price > start);
            }

            if (end > 0.0)
            {
                lots = lots.Where(o => o.AuctionLotItem.Price < end);
            }

            if (!String.IsNullOrEmpty(name))
            {
                lots = lots.Where(o => o.AuctionLotItem.Name.Contains(name));
            }

            switch (sortOrder)
            {
                case AuctionLotSortState.DateDesc:
                    lots = lots.OrderByDescending(s => s.DateAdded);
                    break;
                case AuctionLotSortState.QuantityAsc:
                    lots = lots.OrderBy(s => s.Quantity);
                    break;
                case AuctionLotSortState.QuantityDesc:
                    lots = lots.OrderByDescending(s => s.Quantity);
                    break;

                default:
                    lots = lots.OrderBy(s => s.DateAdded);
                    break;
            }

            count = lots.Count();

            if (page > 0 && rows > 0)
            {
                lots = lots.Skip((page - 1) * rows).Take(rows);
            }

            return lots;
        }


        #endregion
    }
}