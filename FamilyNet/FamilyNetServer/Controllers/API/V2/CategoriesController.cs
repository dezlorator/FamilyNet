using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.Models;
using FamilyNetServer.Validators;
using DataTransferObjects;
using Microsoft.Extensions.Logging;
using FamilyNetServer.HttpHandlers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace FamilyNetServer.Controllers.API.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryValidator _categoryValidator;
        private readonly ILogger<CategoriesController> _logger;
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public CategoriesController(IUnitOfWork unitOfWork,
                                    ICategoryValidator categoryValidator,
                                    ILogger<CategoriesController> logger,
                                    IIdentityExtractor identityExtractor)
        {
            _unitOfWork = unitOfWork;
            _categoryValidator = categoryValidator;
            _logger = logger;
            _identityExtractor = identityExtractor;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery]int rows,
                                    [FromQuery]int page)
        {
            _logger.LogInformation("{info}",
                   "Endpoint Categoties/api/v2 GetAll was called");

            var categories = _unitOfWork.BaseItemTypes.GetAll()
                .Where(c => !c.IsDeleted);

            if (rows > 0 && page > 0)
            {
                _logger.LogInformation("{info}", "Paging was used");

                categories = categories.Skip((page - 1) * rows).Take(rows);
            }

            if (categories == null)
            {
                _logger.LogError("{info}{status}", "List of categories is empty",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var categoriesDTO = await categories.Select(c =>
                new CategoryDTO
                {
                    ID = c.ID,
                    Name = c.Name
                }).ToListAsync();

            _logger.LogInformation("{status}{json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(categoriesDTO));

            return Ok(categoriesDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                 $"Endpoint Categories/api/v2 GetById({id}) was called");

            var category = await _unitOfWork.BaseItemTypes.GetById(id);

            if (category == null)
            {
                _logger.LogError("{info}{status}", $"Category wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var categoryDTO = new CategoryDTO()
            {
                ID = category.ID,
                Name = category.Name
            };

            _logger.LogInformation("{status}{json}", StatusCodes.Status200OK,
               JsonConvert.SerializeObject(categoryDTO));

            return Ok(categoryDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]CategoryDTO categoryDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Categories/api/v2 [POST] was called", userId, token);

            if (!_categoryValidator.IsValid(categoryDTO))
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                    StatusCodes.Status400BadRequest, token, userId,
                    "CategoryDTO is invalid");

                return BadRequest();
            }

            var category = new BaseItemType()
            {
                Name = categoryDTO.Name
            };

            await _unitOfWork.BaseItemTypes.Create(category);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Category was saved [id:{category.ID}]");

            return Created("api/v2/categories/" + category.ID, categoryDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
               "Endpoint Categories/api/v2 [DELETE] was called", userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            var category = await _unitOfWork.BaseItemTypes.GetById(id);

            if (category == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Category was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            category.IsDeleted = true;

            _unitOfWork.BaseItemTypes.Update(category);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"Category.IsDelete was updated [id:{id}]", userId, token);

            return Ok();
        }
    }
}