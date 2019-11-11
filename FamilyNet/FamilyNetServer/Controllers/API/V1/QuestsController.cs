using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.Models;
using FamilyNetServer.Filters;
using FamilyNetServer.Validators;
using DataTransferObjects;
using Microsoft.Extensions.Logging;
using FamilyNetServer.HttpHandlers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class QuestsController : ControllerBase
    {
        #region fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IQuestValidator _questValidator;
        private readonly IQuestsFilter _questsFilter;
        private readonly ILogger<QuestsController> _logger;
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public QuestsController(IUnitOfWork unitOfWork,
                                IQuestValidator questValidator,
                                IQuestsFilter questsFilter,
                                ILogger<QuestsController> logger,
                                IIdentityExtractor identityExtractor)
        {
            _unitOfWork = unitOfWork;
            _questValidator = questValidator;
            _questsFilter = questsFilter;
            _logger = logger;
            _identityExtractor = identityExtractor;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAllAsync([FromQuery]int rows,
                                                    [FromQuery]int page,
                                                    [FromQuery]string forSearch)
        {
            _logger.LogInformation("{info}",
                "Endpoint Quests/api/v1 GetAll was called");

            var quests = _unitOfWork.Quests.GetAll().Where(c => !c.IsDeleted);
            quests = _questsFilter.GetQuests(quests, forSearch);

            if (rows > 0 && page > 0)
            {
                _logger.LogInformation("{info}", "Paging was used");
                quests = quests.Skip((page - 1) * rows).Take(rows);
            }

            if (quests == null)
            {
                _logger.LogInformation("{status}{info}",
                    StatusCodes.Status400BadRequest, "List of Quests is empty");

                return BadRequest();
            }

            var questsDTO = await quests.Select(d =>
                new QuestDTO()
                {
                    ID = d.ID,
                    Name = d.Name,
                    DonationID = d.DonationID,
                    OrphanageID = d.Donation.OrphanageID,
                    DonationItemID = d.Donation.DonationItemID,
                    CharityMakerID = d.Donation.CharityMakerID,
                    VolunteerID = d.VolunteerID
                }).ToListAsync();

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
               JsonConvert.SerializeObject(questsDTO));

            return Ok(questsDTO);
        }

        // GET: api/Quests/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("{info}",
                 $"Endpoint Quests/api/v1 GetById({id}) was called");

            var quest = await _unitOfWork.Quests.GetById(id);

            if (quest == null)
            {
                _logger.LogError("{info}{status}",
                    $"Quest wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var questDTO = new QuestDTO()
            {
                ID = quest.ID,
                Name = quest.Name,
                DonationID = quest.DonationID,
                OrphanageID = quest.Donation.OrphanageID,
                DonationItemID = quest.Donation.DonationItemID,
                CharityMakerID = quest.Donation.CharityMakerID,
                VolunteerID = quest.VolunteerID
            };

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(questDTO));

            return Ok(questDTO);
        }

        // PUT: api/Quests/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, CharityMaker, Volunteer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]QuestDTO questDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Quests/api/v1 [PUT] was called", userId, token);

            if (!_questValidator.IsValid(questDTO))
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                    StatusCodes.Status400BadRequest, token, userId,
                    "QuestDTO is invalid");

                return BadRequest();
            }

            var quest = await _unitOfWork.Quests.GetById(id);

            if (quest == null)
            {
                _logger.LogError("{info}{status}",
                    $"Quest wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            quest.Name = questDTO.Name;

            if (questDTO.VolunteerID != null)
            {
                _logger.LogInformation("{info}", "Volunteer is not null.");
                quest.VolunteerID = questDTO.VolunteerID;
                quest.Status = QuestStatus.Doing;
            }

            if (questDTO.DonationID != null)
            {
                _logger.LogInformation("{info}", "Donation is not null.");
                quest.DonationID = questDTO.DonationID;
            }

            _unitOfWork.Quests.Update(quest);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Quest was edited [id:{quest.ID}]");

            return NoContent();
        }

        // POST: api/Quests
        [HttpPost]
        [Authorize(Roles = "Admin, CharityMaker, Volunteer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm]QuestDTO questDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Quests/api/v1 [POST] was called", userId, token);

            if (!_questValidator.IsValid(questDTO))
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                    StatusCodes.Status400BadRequest, token, userId,
                    "QuestDTO is invalid");

                return BadRequest();
            }

            var quest = new Quest()
            {
                Name = questDTO.Name,
                Description = questDTO.Description,
                VolunteerID = questDTO.VolunteerID,
                DonationID = questDTO.DonationID
            };

            await _unitOfWork.Quests.Create(quest);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{token}{userId}{status}{info}",
                token, userId, StatusCodes.Status201Created,
                $"Quest was saved [id:{quest.ID}]");

            return Created("api/v1/quests/" + quest.ID, questDTO);
        }

        // DELETE: api/Quests/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
               "Endpoint Quests/api/v1 [DELETE] was called", userId, token);

            if (id <= 0)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Argument id is not valid [id:{id}]", userId, token);

                return BadRequest();
            }

            var quest = await _unitOfWork.Quests.GetById(id);

            if (quest == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"Quest was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            quest.IsDeleted = true;

            _unitOfWork.Quests.Update(quest);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK,
                $"Quest.IsDelete was updated [id:{id}]", userId, token);

            return Ok();
        }
    }
}