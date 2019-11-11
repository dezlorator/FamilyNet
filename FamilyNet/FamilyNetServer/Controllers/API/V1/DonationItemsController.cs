using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.Models;
using FamilyNetServer.Validators;
using DataTransferObjects;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using FamilyNetServer.HttpHandlers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DonationItemsController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDonationItemValidator _donationItemValidator;
        private readonly ILogger<DonationItemsController> _logger;
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public DonationItemsController(IUnitOfWork unitOfWork,
                                  IDonationItemValidator donationItemValidator,
                                  ILogger<DonationItemsController> logger,
                                  IIdentityExtractor identityExtractor)
        {
            _unitOfWork = unitOfWork;
            _donationItemValidator = donationItemValidator;
            _logger = logger;
            _identityExtractor = identityExtractor;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("{info}",
                "Endpoint DonationItems/api/v1 GetAll was called");

            var donationItems = _unitOfWork.DonationItems.GetAll()
                .Where(b => !b.IsDeleted);

            if (donationItems == null)
            {
                _logger.LogInformation("{status}{info}",
                    StatusCodes.Status400BadRequest,
                    "List of DonationItems is empty");

                return BadRequest();
            }

            var donationItemsDTO = await donationItems.Select(d =>
                new DonationItemDTO
                {
                    ID = d.ID,
                    Name = d.Name,
                    Description = d.Description,
                    Price = d.Price,
                    CategoriesID = d.TypeBaseItem.Select(t => t.TypeID)
                }).ToListAsync();

            _logger.LogInformation("{status}{json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(donationItemsDTO));

            return Ok(donationItemsDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                $"Endpoint DonationItems/api/v1 GetById({id}) was called");

            var donationItem = await _unitOfWork.DonationItems.GetById(id);

            if (donationItem == null)
            {
                _logger.LogError("{info}{status}", $"DonationItem wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

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

            _logger.LogInformation("{status}{json}",StatusCodes.Status200OK,
                JsonConvert.SerializeObject(donationItemDTO));

            return Ok(donationItemDTO);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Volunteer, Orphan")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]DonationItemDTO donationItemDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint DonationItems/api/v1 [POST] was called", userId, token);

            if (!_donationItemValidator.IsValid(donationItemDTO))
            {
                _logger.LogError("Model is not valid.");

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
                donationItem.TypeBaseItem = donationItemDTO.CategoriesID.Select(c =>
                    new TypeBaseItem
                    {
                        ItemID = ID,
                        TypeID = c
                    }
                ).ToList();

                _logger.LogInformation("{token}{userId}{status}{info}",
                    token, userId, StatusCodes.Status201Created,
                    $"Categories were added.");
            }

            await _unitOfWork.DonationItems.Create(donationItem);
            _unitOfWork.SaveChanges();

            donationItemDTO.ID = donationItem.ID;

            _logger.LogInformation("{token}{userId}{status}{info}",
               token, userId, StatusCodes.Status201Created,
               $"DonationItem was saved [id:{donationItem.ID}]");

            return Created("api/v1/donationItems/" + donationItem.ID, donationItemDTO);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Volunteer, Orphan")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]DonationItemDTO donationItemDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint DonationItems/api/v1 [PUT] was called", userId, token);

            if (!_donationItemValidator.IsValid(donationItemDTO))
            {
                _logger.LogError("{userId} {token} {status} {info}", userId,
                    token, StatusCodes.Status400BadRequest,
                    "DonationItemDTO is invalid");

                return BadRequest();
            }

            var donationItem = await _unitOfWork.DonationItems.GetById(id);

            if (donationItem == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Donation Item was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            donationItem.Name = donationItemDTO.Name;
            donationItem.Description = donationItemDTO.Description;
            donationItem.Price = donationItemDTO.Price;

            _unitOfWork.DonationItems.Update(donationItem);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status204NoContent,
                $"Donation item was updated [id:{donationItem.ID}]");

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
                "Endpoint DonationItems/api/v1 [DELETE] was called",
                userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                   StatusCodes.Status400BadRequest,
                   $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            var donationItem = await _unitOfWork.DonationItems.GetById(id);

            if (donationItem == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Donation Item was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            donationItem.IsDeleted = true;

            _unitOfWork.DonationItems.Update(donationItem);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"DonationItems.IsDelete was updated [id:{id}]", userId, token);

            return Ok();
        }
    }
}