using FamilyNetServer.Enums;
using FamilyNetServer.Filters;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;
using Microsoft.Extensions.Options;
using FamilyNetServer.Configuration;
using FamilyNetServer.Filters.FilterParameters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using FamilyNetServer.Models.EntityFramework;

namespace FamilyNetServer.Controllers.API.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class ChildrenActivitiesController : ControllerBase
    {
        #region private fields

        private readonly EFRepository<ChildActivity> _activityRepository;
        private readonly EFRepository<Award> _awardRepository;
        private readonly ILogger<ChildrenActivitiesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ChildActivityDTO> _childActivityValidator;
        private readonly IFilterConditionsChildrenActivities _filterConditions;

        #endregion

        #region ctor

        public ChildrenActivitiesController(EFRepository<ChildActivity> activityRepository,
                                  EFRepository<Award> awardRepository,
                                  IUnitOfWork unitOfWork,
                                  ILogger<ChildrenActivitiesController> logger,
                                  IValidator<ChildActivityDTO> childActivityValidator,
                                  IFilterConditionsChildrenActivities filterConditions)
        {
            _activityRepository = activityRepository;
            _awardRepository = awardRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _childActivityValidator = childActivityValidator;
            _filterConditions = filterConditions;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]FilterParemetersChildrenActivities filter)
        {
            var activities = _activityRepository.GetAll().Where(a => !a.IsDeleted);
            activities = _filterConditions.GetChildrenActivities(activities, filter);

            if (activities == null)
            {
                _logger.LogInformation("Bad request[400]. Child activity wasn't found.");

                return BadRequest();
            }

            var childActivityDTO = activities.Select(a =>
            new ChildActivityDTO()
            {
                ID = a.ID,
                Name = a.Name,
                Description = a.Description,
                ChildID = a.Child.ID,
                Awards = a.Awards.Select(aw => new AwardDTO
                {
                    ID = aw.ID,
                    Name = aw.Name,
                    Description = aw.Description,
                    Date = aw.Date
                }).ToList()
            });

            _logger.LogInformation("Return Ok[200]. List of children activities was sent");

            return Ok(childActivityDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var activity = await _activityRepository.GetById(id);

            if (activity == null)
            {
                _logger.LogError("Bad request[400]. Child activity wasn't found");

                return BadRequest();
            }

            var childActivityDTO = new ChildActivityDTO()
            {
                ID = activity.ID,
                Name = activity.Name,
                Description = activity.Description,
                ChildID = activity.Child.ID,
                Awards = activity.Awards.Select(aw => new AwardDTO
                {
                    ID = aw.ID,
                    Name = aw.Name,
                    Description = aw.Description,
                    Date = aw.Date
                }).ToList()
            };

            _logger.LogInformation("Return Ok[200]. Child activity was sent.");

            return Ok(childActivityDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]ChildActivityDTO childActivityDTO)
        {
            if (!_childActivityValidator.IsValid(childActivityDTO))
            {
                _logger.LogError("Bad request[400]. ChildActivityDTO is not valid");

                return BadRequest();
            }

            var childActivity = new ChildActivity()
            {
                Name = childActivityDTO.Name,
                Description = childActivityDTO.Description,
                Awards = childActivityDTO.Awards.Select(aw => new Award
                {
                    Name = aw.Name,
                    Description = aw.Description,
                    Date = aw.Date
                }).ToList(),
                Child = await _unitOfWork.Orphans.GetById(childActivityDTO.ChildID)
            };

            if (childActivity.Child == null)
            {
                _logger.LogError("Bad request[400]. Child was not found by id");

                return BadRequest();
            }

            await _activityRepository.Create(childActivity);
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("Return Created[201]. New child activity was added.");

            return Created("api/v2/childrenActivities/" + childActivity.ID, new ChildActivityDTO());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromQuery]int id, [FromBody]ChildActivityDTO childActivityDTO)
        {
            if (!_childActivityValidator.IsValid(childActivityDTO))
            {
                _logger.LogError("Bad request[400]. ChildActivityDTO is not valid");

                return BadRequest();
            }

            var childActivity = await _activityRepository.GetById(childActivityDTO.ID);

            if (childActivity == null)
            {
                _logger.LogError("Bad request[400]. Child activity was not found by id");

                return BadRequest();
            }

            childActivity.Name = childActivityDTO.Name;
            childActivity.Description = childActivityDTO.Description;

            foreach (var a in childActivityDTO.Awards)
            {
                var award = await _awardRepository.GetById(a.ID);

                if (award != null)
                {
                    award.Name = a.Name;
                    award.Description = a.Description;
                    award.Date = a.Date;

                    _awardRepository.Update(award);
                }
                else
                {
                    childActivity.Awards.Add(new Award
                    {
                        Name = a.Name,
                        Description = a.Description,
                        Date = a.Date
                    });
                }
            }

            _activityRepository.Update(childActivity);
            await _awardRepository.SaveChangesAsync();
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("Return NoContent[204]. Child activity was updated.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Bad request[400]. Argument id is not valid");

                return BadRequest();
            }

            var childActivity = await _activityRepository.GetById(id);

            if (childActivity == null)
            {
                _logger.LogError("Bad request[400]. Chidren activity wasn't found.");

                return BadRequest();
            }

            childActivity.IsDeleted = true;

            foreach (var a in childActivity.Awards)
            {
                a.IsDeleted = true;

                _logger.LogInformation("Award property IsDeleted was updated.");
            }

            _activityRepository.Update(childActivity);
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("Return Ok[200]. Child activity property IsDeleted was updated.");

            return Ok();
        }
    }
}