using DataTransferObjects;
using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Identity;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _repository;
        private readonly IValidator<PurchaseDTO> _purchaseValidator;
        private readonly ILogger<PurchaseController> _logger;
        private readonly IFilterConditionPurchase _filterPurchase;

        #endregion

        #region ctor

        public PurchaseController(IUnitOfWork repo,
            IValidator<PurchaseDTO> auctionValidator,
            ILogger<PurchaseController> logger,
            IFilterConditionPurchase filter)
        {
            _repository = repo;
            _purchaseValidator = auctionValidator;
            _logger = logger;
            _filterPurchase = filter;
        }

        #endregion

        [HttpGet]
        [Authorize(Roles = "Admin,CharityMaker, Volunteer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]FilterParamentrsPurchaseDTO filter)
        {
            var purchase = _filterPurchase.GetFiltered(_repository.Purchases.GetAll().Where(c => !c.IsDeleted),
                filter,out var count);
           
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
                    UserId = item.UserId.ToString()
                };

                purchases.Add(purchaseDTO);
            }

            var filterModel = new PurchaseFilterDTO
            {
                PurchaseDTOs = purchases,
                TotalCount = count
            };

            _logger.LogInformation("Returned purchases list");
            return Ok(filterModel);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,CharityMaker, Volunteer")]
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
                UserId = purchase.UserId.ToString()
            };

            _logger.LogInformation($"Returned purchase #{id}");
            return Ok(purchaseDTO);
        }



        [HttpPost]
        [Authorize(Roles = "CharityMaker, Volunteer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]PurchaseDTO purchaseDTO)
        {
            if (!_purchaseValidator.IsValid(purchaseDTO))
            {
                _logger.LogError("Invalid purchase");
                return BadRequest();
            }

            Guid.TryParse(purchaseDTO.UserId, out Guid userId);
            var purchase = new Purchase()
            {
                Date = purchaseDTO.Date,
                AuctionLotId = purchaseDTO.AuctionLotId,
                Paid = purchaseDTO.Paid,
                Quantity = purchaseDTO.Quantity,
                UserId = userId,
                IsDeleted = false
            };

            await _repository.Purchases.Create(purchase);
            _repository.SaveChangesAsync();

            var user = await _repository.UserManager.FindByIdAsync(purchase.UserId.ToString().ToUpper());
            if (user != null)
            {
                var emailSender = new EmailService();

                await emailSender.SendEmailAsync(user.Email, 
                    "Buying crafts", 
                    "<div><h2><b>Thank you for the purchase.</b></h2></div>" +
                    "<h3>Craft info:</h3>" +
                    $"<p>Craft id:< {purchase.AuctionLotId}</p>" +
                    $"<p>Quantity: {purchase.Quantity}</p>" +
                    $"<p>To pay: {purchase.Paid}</p>" +
                    "<h3>Orphanage representatives will contact you♥</h3>");
            }
            purchaseDTO.ID = purchase.ID;

            _logger.LogInformation($"Created purchase with id #{purchase.ID}");

            return Created(purchaseDTO.ID.ToString(), purchaseDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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

            Guid.TryParse(purchaseDTO.UserId, out Guid userId);

            purchase.Date = purchaseDTO.Date;
            purchase.AuctionLotId = purchaseDTO.AuctionLotId;
            purchase.Paid = purchaseDTO.Paid;
            purchase.Quantity = purchaseDTO.Quantity;
            purchase.UserId = userId;

            _repository.Purchases.Update(purchase);
            _repository.SaveChangesAsync();

            _logger.LogInformation($"Edited purchase with id #{purchase.ID}");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
