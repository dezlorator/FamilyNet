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

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CategoryDTO> _categoryValidator;
        private readonly ILogger<CategoriesController> _logger;

        #endregion

        #region ctor

        public CategoriesController(IUnitOfWork unitOfWork,
                                    IValidator<CategoryDTO> categoryValidator,
                                    ILogger<CategoriesController> logger)
        {
            _unitOfWork = unitOfWork;
            _categoryValidator = categoryValidator;
            _logger = logger;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]int rows,
                                    [FromQuery]int page)
        {
            var categories = _unitOfWork.BaseItemTypes.GetAll().Where(c => !c.IsDeleted);

            if (rows > 0 && page > 0)
            {
                _logger.LogInformation("Paging were used");
                categories = categories
                    .Skip((page - 1) * rows).Take(rows);
            }

            if (categories == null)
            {
                _logger.LogError("Bad request. No categories were found");
                return BadRequest();
            }

            var categoriesDTO = new List<CategoryDTO>();

            categoriesDTO = categories.Select(c =>
                new CategoryDTO
                {
                    ID = c.ID,
                    Name = c.Name
                }).ToList();

            _logger.LogInformation("Status: OK. List of categories was sent");

            return Ok(categoriesDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _unitOfWork.BaseItemTypes.GetById(id);

            if (category == null)
            {
                _logger.LogError("Bad request. No category was found");
                return BadRequest();
            }

            var categoryDTO = new CategoryDTO()
            {
                ID = category.ID,
                Name = category.Name
            };

            _logger.LogInformation("Status: OK. Category item was sent");

            return Ok(categoryDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]CategoryDTO categoryDTO)
        {
            if (!_categoryValidator.IsValid(categoryDTO))
            {
                _logger.LogError("Model is not valid.");
                return BadRequest();
            }

            var category = new BaseItemType()
            {
                Name = categoryDTO.Name
            };

            await _unitOfWork.BaseItemTypes.Create(category);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("Status: Created. Category was created");

            return Created("api/v1/categories/" + category.ID, categoryDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Bad request. Id must be greater than zero.");
                return BadRequest();
            }

            var category = await _unitOfWork.BaseItemTypes.GetById(id);

            if (category == null)
            {
                _logger.LogError("Bad request. No category with such id was found");
                return BadRequest();
            }

            category.IsDeleted = true;

            _unitOfWork.BaseItemTypes.Update(category);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("Status: OK. Category was deleted.");

            return Ok();
        }
    }
}