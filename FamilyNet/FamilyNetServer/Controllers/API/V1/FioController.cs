using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects.Enums;
using DataTransferObjects;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FioController : ControllerBase
    {
        #region
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FioController> _logger;
        #endregion

        public FioController(IUnitOfWork unitOfWork, ILogger<FioController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById(int id, [FromQuery]UserRole role)
        {
            Person person = null;

            switch (role)
            {
                case UserRole.CharityMaker:
                    person = await _unitOfWork.CharityMakers.GetById(id);
                    break;
                case UserRole.Orphan:
                    person = await _unitOfWork.Orphans.GetById(id);
                    break;
                case UserRole.Representative:
                    person = await _unitOfWork.Representatives.GetById(id);
                    break;
                case UserRole.Volunteer:
                    person = await _unitOfWork.Volunteers.GetById(id);
                    break;
                default:
                    _logger.LogWarning(string.Format("This role has no Fio {0}",
                        nameof(role)));
                    return Forbid();
            }

            if (person == null)
            {
                return BadRequest();
            }

            var fioDTO = new FioDTO()
            {
                Name = person.FullName.Name,
                Surname = person.FullName.Surname,
                Patronymic = person.FullName.Patronymic
            };

            _logger.LogInformation(string.Format("{0} fio with id {1} was sent",
                nameof(role), id));
            return Ok(fioDTO);
        }
    }
}