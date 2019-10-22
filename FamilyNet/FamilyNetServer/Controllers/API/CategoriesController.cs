using DataTransferObjects;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ICategoryValidator _categoryValidator;

        #endregion

        public CategoriesController(IUnitOfWorkAsync unitOfWork,
                                    ICategoryValidator categoryValidator)
        {
            _unitOfWork = unitOfWork;
            _categoryValidator = categoryValidator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]int rows,
                                    [FromQuery]int page)
        {
            var categories = _unitOfWork.BaseItemTypes.GetAll().Where(c => !c.IsDeleted);

            if (rows != 0 && page != 0)
            {
                categories = categories
                    .Skip((page - 1) * rows).Take(rows);
            }

            if (categories == null)
            {
                return BadRequest();
            }

            var categoriesDTO = new List<CategoryDTO>();

            foreach (var c in categories)
            {
                categoriesDTO.Add(new CategoryDTO
                {
                    Name = c.Name
                }
                );
            }

            return Ok(categoriesDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]CategoryDTO categoryDTO)
        {
            if (!_categoryValidator.IsValid(categoryDTO))
            {
                return BadRequest();
            }

            var category = new BaseItemType()
            {
                Name = categoryDTO.Name
            };

            await _unitOfWork.BaseItemTypes.Create(category);
            _unitOfWork.SaveChangesAsync();

            return Created("api/v1/categories/" + category.ID, categoryDTO);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var category = await _unitOfWork.BaseItemTypes.GetById(id);

            if (category == null)
            {
                return BadRequest();
            }

            category.IsDeleted = true;

            _unitOfWork.BaseItemTypes.Update(category);
            _unitOfWork.SaveChangesAsync();

            return Ok();
        }
    }
}