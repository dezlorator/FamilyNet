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
using Newtonsoft.Json;
using FamilyNetServer.HttpHandlers;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
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
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public ChildrenActivitiesController(EFRepository<ChildActivity> activityRepository,
                                  EFRepository<Award> awardRepository,
                                  IUnitOfWork unitOfWork,
                                  ILogger<ChildrenActivitiesController> logger,
                                  IValidator<ChildActivityDTO> childActivityValidator,
                                  IFilterConditionsChildrenActivities filterConditions,
                                  IIdentityExtractor identityExtractor)
        {
            _activityRepository = activityRepository;
            _awardRepository = awardRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _childActivityValidator = childActivityValidator;
            _filterConditions = filterConditions;
            _identityExtractor = identityExtractor;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll([FromQuery]FilterParemetersChildrenActivities filter)
        {
            _logger.LogInformation("{info}",
                "Endpoint ChildrenActivities/api/v1 GetAll was called");

            var activities = _activityRepository.GetAll().Where(a => !a.IsDeleted);
            activities = _filterConditions.GetChildrenActivities(activities, filter);

            if (activities == null)
            {
                _logger.LogInformation("{status}{info}",
                    StatusCodes.Status400BadRequest,
                    "List of ChildrenActivities is empty");

                return BadRequest();
            }

            var childrenActivitiesDTO = activities.Select(activity =>
            new ChildActivityDTO()
            {
                ID = activity.ID,
                Name = activity.Name,
                Description = activity.Description,
                ChildID = activity.Child.ID,
                Awards = activity.Awards.Where(award => !award.IsDeleted).Select(award => new AwardDTO
                {
                    ID = award.ID,
                    Name = award.Name,
                    Description = award.Description,
                    Date = award.Date
                }).ToList()
            });

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(childrenActivitiesDTO));

            return Ok(childrenActivitiesDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                $"Endpoint ChildrenActivities/api/v1 GetById({id}) was called");

            var activity = await _activityRepository.GetById(id);

            if (activity == null)
            {
                _logger.LogError("{info}{status}",
                    $"ChildActivity wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var childActivityDTO = new ChildActivityDTO()
            {
                ID = activity.ID,
                Name = activity.Name,
                Description = activity.Description,
                ChildID = activity.Child.ID,
                Awards = activity.Awards.Where(award => !award.IsDeleted).Select(award => new AwardDTO
                {
                    ID = award.ID,
                    Name = award.Name,
                    Description = award.Description,
                    Date = award.Date
                }).ToList()
            };

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(childActivityDTO));

            return Ok(childActivityDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]ChildActivityDTO childActivityDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint ChildrenActivities/api/v1 [POST] was called", userId, token);

            if (!_childActivityValidator.IsValid(childActivityDTO))
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                    StatusCodes.Status400BadRequest, token, userId,
                    "ChildActivityDTO is invalid");

                return BadRequest();
            }

            var childActivity = new ChildActivity()
            {
                Name = childActivityDTO.Name,
                Description = childActivityDTO.Description,
                Child = await _unitOfWork.Orphans.GetById(childActivityDTO.ChildID)
            };

            if (childActivityDTO.Awards != null)
            {
                childActivity.Awards = childActivityDTO.Awards.Select(award => new Award
                {
                    Name = award.Name,
                    Description = award.Description,
                    Date = award.Date
                }).ToList();
            }

            if (childActivity.Child == null)
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                    StatusCodes.Status400BadRequest, token, userId,
                    "Child wasn't found by id.");

                return BadRequest();
            }

            await _activityRepository.Create(childActivity);
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"ChildActivity was saved [id:{childActivity.ID}]");

            return Created("api/v1/childrenActivities/" + childActivity.ID, new ChildActivityDTO());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromQuery]int id, [FromForm]ChildActivityDTO childActivityDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint ChildrenActivities/api/v1 [PUT] was called", userId, token);

            if (!_childActivityValidator.IsValid(childActivityDTO))
            {
                _logger.LogError("{userId} {token} {status} {info}",
                    userId, token, StatusCodes.Status400BadRequest,
                    "ChildActivityDTO is invalid");

                return BadRequest();
            }

            var childActivity = await _activityRepository.GetById(childActivityDTO.ID);

            if (childActivity == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"ChildActivity was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            childActivity.Name = childActivityDTO.Name;
            childActivity.Description = childActivityDTO.Description;

            if (childActivityDTO.Awards != null)
            {
                foreach (var award in childActivity.Awards)
                {
                    if (childActivityDTO.Awards.FirstOrDefault(i => i.ID == award.ID) == null)
                    {
                        award.IsDeleted = true;

                        _logger.LogInformation("{info}{userId}{token}",
                            "Award.IsDeleted was updated.",
                            userId, token);
                    }
                }

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
            }

            _activityRepository.Update(childActivity);
            await _awardRepository.SaveChangesAsync();
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("{token}{userId}{status}{info}",
                 token, userId, StatusCodes.Status204NoContent,
                 $"ChildActivity was updated [id:{childActivity.ID}]");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint ChildrenActivities/api/v1 [DELETE] was called",
                userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            var childActivity = await _activityRepository.GetById(id);

            if (childActivity == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"ChildActivity was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            childActivity.IsDeleted = true;

            foreach (var award in childActivity.Awards)
            {
                award.IsDeleted = true;

                _logger.LogInformation("{info}{userId}{token}",
                    "Award.IsDeleted was updated.",
                    userId, token);
            }

            _activityRepository.Update(childActivity);
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"ChildActivity.IsDeleted was updated [id:{id}]", userId, token);

            return Ok();
        }
    }
}