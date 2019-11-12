using DataTransferObjects;
using FamilyNetServer.Filters;
using FamilyNetServer.HttpHandlers;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Identity;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
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
        private readonly IIdentityExtractor _identityExtractor;
        private readonly IFilterConditionPurchase _filterPurchase;

        #endregion

        #region ctor

        public PurchaseController(IUnitOfWork repo,
            IValidator<PurchaseDTO> auctionValidator,
            ILogger<PurchaseController> logger,
            IIdentityExtractor identityExtractor,
            IFilterConditionPurchase filter)
        {
            _repository = repo;
            _purchaseValidator = auctionValidator;
            _logger = logger;
            _identityExtractor = identityExtractor;
            _filterPurchase = filter;

        }

        #endregion

        [HttpGet]
        [Authorize(Roles = "Admin, CharityMaker, Volunteer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery]FilterParamentrsPurchaseDTO filter)
        {
            var userIdentity = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{token}{userId}",
                "Endpoint Purchase/api/v1 GetAll was called", token, userIdentity);

            var purchase = _filterPurchase.GetFiltered(_repository.Purchases.GetAll().Where(c => !c.IsDeleted),
                filter, out var count);

            if (purchase == null)
            {
                _logger.LogInformation("{status}{info}",
                    StatusCodes.Status400BadRequest,
                    "List of Purchases is empty");
                return BadRequest();
            }

            var purchases =  purchase.Select(item =>
                new PurchaseDTO()
                {
                    ID = item.ID,
                    Date = item.Date,
                    AuctionLotId = item.AuctionLotId,
                    Paid = item.Paid,
                    Quantity = item.Quantity,
                    UserId = item.UserId.ToString()
                }).ToList();

            var filterModel = new PurchaseFilterDTO
            {
                PurchaseDTOs = purchases,
                TotalCount = count
            };
                      
            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(purchases));

            return Ok(filterModel);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,CharityMaker, Volunteer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var userIdentity = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{token}{userId}",
                 $"Endpoint Purchase/api/v1 GetById({id}) was called",
                 token, userIdentity);

            var purchase = await _repository.Purchases.GetById(id);

            if (purchase == null)
            {
                _logger.LogError("{info}{status}",
                    $"Purchase wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

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

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(purchaseDTO));

            return Ok(purchaseDTO);
        }



        [HttpPost]
        [Authorize(Roles = "CharityMaker, Volunteer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]PurchaseDTO purchaseDTO)
        {
            var userIdentity = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Purchase/api/v1 [POST] was called", userIdentity, token);

            if (!_purchaseValidator.IsValid(purchaseDTO))
            {
                _logger.LogWarning("{status}{token}{userId}",
                    StatusCodes.Status400BadRequest, token, userIdentity);

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
            _repository.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Purchase was saved [id:{purchase.ID}]");

            var user = await _repository.UserManager
                .FindByIdAsync(purchase.UserId.ToString().ToUpper());

            if (user != null)
            {
                var emailSender = new EmailService();

               await emailSender.SendEmailAsync(user.Email, 
                    "Buying crafts", 
                    "<div><h2><b>Thank you for the purchase.</b></h2></div>" +
                    "<h3>Craft info:</h3>" +
                    $"<p>Craft id: {purchase.AuctionLotId}</p>" +
                    $"<p>Quantity: {purchase.Quantity}</p>" +
                    $"<p>To pay: {purchase.Paid}</p>" +
                    "<h3>Orphanage representatives will contact you♥</h3>");

            }
            purchaseDTO.ID = purchase.ID;

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Purchase was saved [id:{purchase.ID}]");

            return Created(purchaseDTO.ID.ToString(), purchaseDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromForm]PurchaseDTO purchaseDTO)
        {
            var userIdentity = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Purchase/api/v1 [PUT] was called", userIdentity, token);

            if (!_purchaseValidator.IsValid(purchaseDTO))
            {
                _logger.LogError("{userId} {token} {status} {info}",
                    userIdentity, token, StatusCodes.Status400BadRequest,
                    "Purchase enity is invalid");

                return BadRequest();
            }

            var purchase = await _repository.Purchases.GetById(id);

            if (purchase == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Purchase was not found [id:{id}]", userIdentity, token);

                return BadRequest();
            }

            Guid.TryParse(purchaseDTO.UserId, out Guid userId);

            purchase.Date = purchaseDTO.Date;
            purchase.AuctionLotId = purchaseDTO.AuctionLotId;
            purchase.Paid = purchaseDTO.Paid;
            purchase.Quantity = purchaseDTO.Quantity;
            purchase.UserId = userId;

            _repository.Purchases.Update(purchase);
            _repository.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status204NoContent,
                $"Purchase was updated [id:{purchase.ID}]");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
               "Endpoint Purchase/api/v1 [DELETE] was called", userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Purchase was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            var purchase = await _repository.Purchases.GetById(id);

            if (purchase == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            purchase.IsDeleted = true;

            _repository.Purchases.Update(purchase);
            _repository.SaveChanges();

            _logger.LogInformation("{status} {info} {userId} {token}",
               StatusCodes.Status200OK,
               $"Purchase.IsDelete was updated [id:{id}]",
               userId, token);

            return Ok();
        }
    }
}