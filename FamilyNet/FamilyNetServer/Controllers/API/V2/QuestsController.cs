using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.Models;
using FamilyNetServer.Filters;
using FamilyNetServer.Validators;
using DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;

namespace FamilyNetServer.Controllers.API.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class QuestsController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<QuestDTO> _questValidator;
        private readonly IQuestsFilter _questsFilter;
        private readonly ILogger<QuestsController> _logger;

        #endregion

        public QuestsController(IUnitOfWork unitOfWork,
                                IValidator<QuestDTO> questValidator,
                                IQuestsFilter questsFilter,
                                ILogger<QuestsController> logger)
        {
            _unitOfWork = unitOfWork;
            _questValidator = questValidator;
            _questsFilter = questsFilter;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult GetAll([FromQuery]int rows,
                                   [FromQuery]int page,
                                   [FromQuery]string forSearch,
                                   [FromQuery]string status = "ToDo")
        {
            var quests = _unitOfWork.Quests.GetAll().Where(c => !c.IsDeleted);

            if (!System.Enum.TryParse(status, out QuestStatus questStatus))
            {
                _logger.LogError("Bad request. Could not parse status. ");
                return BadRequest();
            }

            quests = _questsFilter.GetQuests(quests, forSearch, questStatus);

            if (rows > 0 && page > 0)
            {
                _logger.LogInformation("Paging were used");
                quests = quests
                    .Skip((page - 1) * rows).Take(rows);
            }

            if (quests == null)
            {
                _logger.LogError("Bad request. No quests were found");
                return BadRequest();
            }

            var questsDTO = new List<QuestDTO>();

            questsDTO = quests.Select(d =>
                new QuestDTO()
                {
                    ID = d.ID,
                    Name = d.Name,
                    Description = d.Description,
                    Status = d.Status.ToString(),
                    DonationID = d.DonationID,
                    OrphanageID = d.Donation.OrphanageID,
                    OrphanageName = d.Donation.Orphanage.Name,
                    CharityMakerID = d.Donation.CharityMakerID,
                    DonationName = d.Donation.DonationItem.Name,
                    DonationDescription = d.Donation.DonationItem.Description,
                    VolunteerID = d.VolunteerID,
                    FromDate = d.FromDate,
                    ToDate = d.ToDate
                }).ToList();

            _logger.LogInformation("Status: OK. List of quests was sent");
            return Ok(questsDTO);
        }

        // GET: api/Quests/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var quest = await _unitOfWork.Quests.GetById(id);

            if (quest == null)
            {
                _logger.LogError("Bad request. No quest was found");
                return BadRequest();
            }

            var questDTO = new QuestDTO()
            {
                ID = quest.ID,
                Name = quest.Name,
                Description = quest.Description,
                DonationID = quest.DonationID,
                VolunteerID = quest.VolunteerID,
                FromDate = quest.FromDate,
                ToDate = quest.ToDate,
                Status = quest.Status.ToString()
            };

            if (questDTO.DonationID != null)
            {
                var donation = await _unitOfWork.Donations.GetById(questDTO.DonationID.Value);
                var orphanage = await _unitOfWork.Orphanages.GetById(donation.OrphanageID.Value);
                var item = await _unitOfWork.DonationItems.GetById(donation.DonationItemID.Value);

                questDTO.OrphanageID = donation.OrphanageID;
                questDTO.OrphanageName = orphanage.Name;
                questDTO.CharityMakerID = donation.CharityMakerID;
                questDTO.DonationName = item.Name;
                questDTO.DonationDescription = item.Description;
            }

            _logger.LogInformation("Status: OK. Quest was sent");
            return Ok(questDTO);
        }

        // PUT: api/Quests/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Volunteer, CharityMaker, Representative, Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]QuestDTO questDTO)
        {
            if (!_questValidator.IsValid(questDTO))
            {
                _logger.LogError("Model is not valid.");
                return BadRequest();
            }

            var quest = await _unitOfWork.Quests.GetById(id);

            if (quest == null)
            {
                _logger.LogError("Bad request. No quest was found");
                return BadRequest();
            }

            quest.Name = questDTO.Name;
            quest.Description = questDTO.Description;

            if (!Enum.TryParse(questDTO.Status, out QuestStatus status))
            {
                return BadRequest();
            }

            quest.Status = status;

            if (questDTO.VolunteerID != quest.VolunteerID)
            {
                _logger.LogInformation("Volunteer is not null.");
                quest.VolunteerID = questDTO.VolunteerID;
                quest.Status = QuestStatus.Doing;
            }

            if (questDTO.DonationID != null)
            {
                _logger.LogInformation("Donation is not null.");
                quest.DonationID = questDTO.DonationID;
            }

            _unitOfWork.Quests.Update(quest);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("Status: NoContent. Quest was edited.");

            return NoContent();
        }

        // POST: api/Quests
        [HttpPost]
        [Authorize(Roles = "Volunteer, CharityMaker, Representative, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]QuestDTO questDTO)
        {
            if (!_questValidator.IsValid(questDTO))
            {
                _logger.LogError("Model is not valid.");
                return BadRequest();
            }

            var quest = new Quest()
            {
                Name = questDTO.Name,
                Description = questDTO.Description,
                DonationID = questDTO.DonationID,
                FromDate = questDTO.FromDate,
                ToDate = questDTO.ToDate,
                Status = QuestStatus.ToDo
            };

            if (questDTO.DonationID != null)
            {
                var donation = await _unitOfWork.Donations.GetById(questDTO.DonationID.Value);
                var orphanage = await _unitOfWork.Orphanages.GetById(donation.OrphanageID.Value);
                var item = await _unitOfWork.DonationItems.GetById(donation.DonationItemID.Value);

                questDTO.OrphanageName = orphanage.Name;
                questDTO.CharityMakerID = donation.CharityMakerID;
                questDTO.DonationName = item.Name;
                questDTO.DonationDescription = item.Description;
            }

            await _unitOfWork.Quests.Create(quest);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("Status: Created. Quest was created");
            return Created("api/v1/quests/" + quest.ID, questDTO);
        }

        // DELETE: api/Quests/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Volunteer, CharityMaker, Representative, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Bad request. Id must be greater than zero.");
                return BadRequest();
            }

            var quest = await _unitOfWork.Quests.GetById(id);

            if (quest == null)
            {
                _logger.LogError("Bad request. No quest with such id was found");
                return BadRequest();
            }

            quest.IsDeleted = true;

            _unitOfWork.Quests.Update(quest);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("Status: OK. Quest was deleted.");

            return Ok();
        }
    }
}