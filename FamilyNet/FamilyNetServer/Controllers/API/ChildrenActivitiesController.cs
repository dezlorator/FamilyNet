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

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ChildrenActivitiesController : ControllerBase
    {
        #region private fields

        private readonly EFRepository<ChildrenActivity> _activityRepository;
        private readonly EFRepository<Award> _awardRepository;
        private readonly IOptionsSnapshot<ServerURLSettings> _settings;
        private readonly ILogger<ChildrenActivitiesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChildrenActivityValidator _childrenActivityValidator;

        #endregion

        #region ctor

        public ChildrenActivitiesController(EFRepository<ChildrenActivity> activityRepository,
                                  EFRepository<Award> awardRepository,
                                  IUnitOfWork unitOfWork,
                                  IOptionsSnapshot<ServerURLSettings> setings,
                                  ILogger<ChildrenActivitiesController> logger,
                                  IChildrenActivityValidator childrenActivityValidator)
        {
            _activityRepository = activityRepository;
            _awardRepository = awardRepository;
            _unitOfWork = unitOfWork;
            _settings = setings;
            _logger = logger;
            _childrenActivityValidator = childrenActivityValidator;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll()
        {
            var activities = _activityRepository.GetAll();

            if (activities == null)
            {
                return BadRequest();
            }

            var childrenActivityDTO = activities.Select(a =>
            new ChildrenActivityDTO()
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

            return Ok(childrenActivityDTO);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var activity = await _activityRepository.GetById(id);

            if (activity == null)
            {
                return BadRequest();
            }

            var childrenActivityDTO = new ChildrenActivityDTO()
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

            return Ok(childrenActivityDTO);
        }

        [HttpPost]
       // [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]ChildrenActivityDTO childrenActivityDTO)
        {
            if (!_childrenActivityValidator.IsValid(childrenActivityDTO))
            {
                return BadRequest();
            }

            var childrenActivity = new ChildrenActivity()
            {
                Name = childrenActivityDTO.Name,
                Description = childrenActivityDTO.Description,
                Awards = childrenActivityDTO.Awards.Select(aw => new Award
                {
                    Name = aw.Name,
                    Description = aw.Description,
                    Date = aw.Date
                }).ToList(),
                Child = await _unitOfWork.Orphans.GetById(childrenActivityDTO.ChildID)   
            };

            if (childrenActivity.Child == null)
            {
                return BadRequest();
            }

            await _activityRepository.Create(childrenActivity);
            await _activityRepository.SaveChangesAsync();

            return Created("api/v1/childrenActivities/" + childrenActivity.ID, new ChildrenActivityDTO());
        }

        [HttpPut("{id}")]
       // [Authorize(Roles = "Admin, Representative")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit([FromQuery]int id, [FromForm]ChildrenActivityDTO childrenActivityDTO)
        {
            if (!_childrenActivityValidator.IsValid(childrenActivityDTO))
            {
                return BadRequest();
            }

            var childrenActivity = await _activityRepository.GetById(childrenActivityDTO.ID);

            if (childrenActivity == null)
            {
                return BadRequest();
            }

            childrenActivity.Name = childrenActivityDTO.Name;
            childrenActivity.Description = childrenActivityDTO.Description;

            foreach (var a in childrenActivityDTO.Awards)
            {
                var award = await _awardRepository.GetById(a.ID);

                if(award!=null)
                {
                    award.Name = a.Name;
                    award.Description = a.Description;
                    award.Date = a.Date;

                    _awardRepository.Update(award);
                }
                else
                {
                    childrenActivity.Awards.Add(new Award
                    {
                        Name = a.Name,
                        Description = a.Description,
                        Date = a.Date
                    });
                }
            }

            _activityRepository.Update(childrenActivity);
            await _awardRepository.SaveChangesAsync();
            await _activityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var childrenActivity = await _activityRepository.GetById(id);

            if (childrenActivity == null)
            {
                return BadRequest();
            }

            childrenActivity.IsDeleted = true;

            foreach (var a in childrenActivity.Awards)
            {
                a.IsDeleted = true;
            }

            _activityRepository.Update(childrenActivity);
            await _activityRepository.SaveChangesAsync();

            return Ok();
        }
    }
}