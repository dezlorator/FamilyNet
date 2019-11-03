using DataTransferObjects;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _repository;
        private readonly IValidator<PurchaseDTO> _purchaseValidator;
        private readonly ILogger<PurchaseController> _logger;

        #endregion

        #region ctor

        public PurchaseController(IUnitOfWork repo,
            IValidator<PurchaseDTO> auctionValidator,
            ILogger<PurchaseController> logger)
        {
            _repository = repo;
            _purchaseValidator = auctionValidator;
            _logger = logger;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll()
        {
            var purchase = _repository.Purchases.GetAll().Where(c => !c.IsDeleted);

            if (purchase == null)
            {
                return BadRequest();
            }

            var purchases = new List<PurchaseDTO>();

            foreach (var item in purchase)
            {
                var purchaseDTO = new PurchaseDTO()
                {
                    ID = item.ID,
                    Date = item.Date,
                    AuctionLotId = item.AuctionLotId,
                    Paid = item.Paid,
                    Quantity = item.Quantity,
                    UserId = item.UserId
                };

                purchases.Add(purchaseDTO);
            }

            _logger.LogInformation("Returned purchases list");
            return Ok(purchases);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var purchase = await _repository.Purchases.GetById(id);

            if (purchase == null)
            {
                _logger.LogError($"No purchase with id #{id} in database");
                return BadRequest();
            }

            var purchaseDTO = new PurchaseDTO()
            {
                ID = purchase.ID,
                Date = purchase.Date,
                AuctionLotId = purchase.AuctionLotId,
                Paid = purchase.Paid,
                Quantity = purchase.Quantity,
                UserId = purchase.UserId
            };

            _logger.LogInformation($"Returned purchase #{id}");
            return Ok(purchaseDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]PurchaseDTO purchaseDTO)
        {
            if (!_purchaseValidator.IsValid(purchaseDTO))
            {
                _logger.LogError("Invalid purchase");
                return BadRequest();
            }

            var purchase = new Purchase()
            {
                Date = purchaseDTO.Date,
                AuctionLotId = purchaseDTO.AuctionLotId,
                Paid = purchaseDTO.Paid,
                Quantity = purchaseDTO.Quantity,
                UserId = purchaseDTO.UserId,
                IsDeleted = false
            };

            await _repository.Purchases.Create(purchase);
            _repository.SaveChangesAsync();

            purchaseDTO.ID = purchase.ID;

            _logger.LogInformation($"Created purchase with id #{purchase.ID}");

            return Created(purchaseDTO.ID.ToString(), purchaseDTO);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromForm]PurchaseDTO purchaseDTO)
        {
            if (!_purchaseValidator.IsValid(purchaseDTO))
            {
                _logger.LogError("Invalid purchase");
                return BadRequest();
            }

            var purchase = await _repository.Purchases.GetById(id);

            if (purchase == null)
            {
                return BadRequest();
            }

            purchase.Date = purchaseDTO.Date;
            purchase.AuctionLotId = purchaseDTO.AuctionLotId;
            purchase.Paid = purchaseDTO.Paid;
            purchase.Quantity = purchaseDTO.Quantity;
            purchase.UserId = purchaseDTO.UserId;

            _repository.Purchases.Update(purchase);
            _repository.SaveChangesAsync();

            _logger.LogInformation($"Edited purchase with id #{purchase.ID}");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"No purchase with id #{id} in database");
                return BadRequest();
            }

            var purchase = await _repository.Purchases.GetById(id);

            if (purchase == null)
            {
                _logger.LogError($"No purchase with id #{id} in database");
                return BadRequest();
            }

            purchase.IsDeleted = true;

            _repository.Purchases.Update(purchase);
            _repository.SaveChangesAsync();

            _logger.LogInformation($"Deleted auction lot with id #{purchase.ID}");

            return Ok();
        }
    }
}
