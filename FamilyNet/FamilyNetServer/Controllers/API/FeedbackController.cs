using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.Models;
using FamilyNetServer.Models.EntityFramework;
using FamilyNetServer.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DataTransferObjects;
using Microsoft.Extensions.Options;
using FamilyNetServer.Configuration;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Enums;
using Microsoft.AspNetCore.Authorization;
using DataTransferObjects.Enums;
using FamilyNetServer.EnumConvertor;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        #region private
        private readonly EFRepository<Feedback> _feedbackRepository;
        private readonly ILogger<FeedbackController> _logger;
        private readonly IFeedbackValidator _validator;
        private readonly IFileUploader _fileUploader;
        private readonly IOptionsSnapshot<ServerURLSettings> _settings;
        private readonly IConvertUserRole _userRoleConvertor;
        #endregion

        public FeedbackController(EFRepository<Feedback> feedbackRepository,
            ILogger<FeedbackController> logger, IFeedbackValidator validator,
            IOptionsSnapshot<ServerURLSettings> settings,
            IFileUploader fileUploader,
            IConvertUserRole userRoleConvertor)
        {
            _feedbackRepository = feedbackRepository;
            _logger = logger;
            _validator = validator;
            _settings = settings;
            _fileUploader = fileUploader;
            _userRoleConvertor = userRoleConvertor;
        }

        [HttpGet("donation/{donationId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetByDonationId(int donationId)
        {
            var feedbackContainer = _feedbackRepository.GetAll().Where(p => p.DonationId == donationId
            && p.IsDeleted == false);

            if (feedbackContainer == null)
            {
                _logger.LogError(string.Format("No feedback found with such donation id - {0}",
                    donationId));
                return BadRequest();
            }

            var feedbackDTO = new List<FeedbackDTO>();

            foreach (var feedback in feedbackContainer)
            {
                feedbackDTO.Add(new FeedbackDTO
                {
                    ID = feedback.ID,
                    DonationId = feedback.DonationId,
                    ImagePath = _settings.Value.ServerURL + feedback.Image,
                    Message = feedback.Message,
                    Rating = feedback.Rating,
                    ReceiverId = feedback.ReceiverId,
                    ReceiverRole = feedback.ReceiverRole,
                    Time = feedback.Time,
                    SenderId = feedback.SenderId ?? 0,
                    SenderRole = feedback.SenderRole
                });
            }

            _logger.LogInformation(string.Format("List of feedback with such donation id -{0} was sent",
                donationId));
            return Ok(feedbackDTO);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            if(id < 0)
            {
                _logger.LogError("Id should be bigger than -1 id = {0}", id);
                return BadRequest();
            }

            var feedback = await _feedbackRepository.GetById(id);

            if(feedback == null)
            {
                _logger.LogError(string.Format("No feedback found with such donation id - {0}",
                id));
                return BadRequest();
            }

            var feedbackDTO = new FeedbackDTO()
            {
                ID = feedback.ID,
                DonationId = feedback.DonationId,
                ImagePath = _settings.Value.ServerURL + feedback.Image,
                Message = feedback.Message,
                Rating = feedback.Rating,
                ReceiverId = feedback.ReceiverId,
                ReceiverRole = feedback.ReceiverRole,
                Time = feedback.Time,
                SenderId = feedback.SenderId ?? 0,
                SenderRole = feedback.SenderRole
            };

            _logger.LogInformation(string.Format("Feedback with id - {0} was sent", id));
            return Ok(feedbackDTO);
        }

        [HttpPost]
        [Authorize(Roles = "CharityMaker, Volunteer, Representative")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] FeedbackDTO feedbackDTO)
        {
            string errorMessage = string.Empty;

            if(!_validator.IsValid(feedbackDTO, ref errorMessage))
            {
                _logger.LogError(errorMessage);
                return BadRequest();
            }

            //how it will be better???
            var claims = HttpContext.User.Claims.ToList();
            int senderId = Convert.ToInt32(claims[2].Value);
            var senderRole = _userRoleConvertor.ConvertFromString(claims[3].Value);

            if(!_validator.CheckPermission(senderRole, feedbackDTO.ReceiverRole))
            {
                _logger.LogError(string.Format("User with id - {0} have no permission to leave" +
                    "feedback about {1}", claims[0].Value, nameof(feedbackDTO.ReceiverRole)));
                //what should I return
                return BadRequest();
            }

            string photoPath = string.Empty;

            if(feedbackDTO.Image != null)
            {
                var fileName = DateTime.Now.Ticks.ToString();

                photoPath = _fileUploader.CopyFileToServer(fileName,
                    nameof(DirectoryUploadName.Feedback), feedbackDTO.Image);
                _logger.LogInformation(string.Format("{0} - this path to photo was created", photoPath));
            }

            var feedback = new Feedback()
            {
                DonationId = feedbackDTO.DonationId,
                Image = _settings.Value.ServerURL + photoPath,
                Message = feedbackDTO.Message,
                Rating = feedbackDTO.Rating,
                ReceiverId = feedbackDTO.ReceiverId,
                ReceiverRole = feedbackDTO.ReceiverRole,
                Time = feedbackDTO.Time,
                SenderId = senderId,
                SenderRole = senderRole
            };

            await _feedbackRepository.Create(feedback);
            await _feedbackRepository.SaveChangesAsync();

            _logger.LogInformation(string.Format("Feedback was created, date{0}", feedbackDTO.Time));
            return Created("api/v1/charityMakers/" + feedbackDTO.DonationId, feedback);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CharityMaker, Volunteer, Representative")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, [FromForm]FeedbackDTO feedbackDTO)
        {
            if(id < 0)
            {
                _logger.LogError("Id should be bigger than -1 id = {0}", id);
                return BadRequest();
            }

            var feedback = await _feedbackRepository.GetById(id);

            var claims = HttpContext.User.Claims.ToList();
            int editorId = Convert.ToInt32(claims[2].Value);
            var editorRole = _userRoleConvertor.ConvertFromString(claims[3].Value);

            if (!_validator.CheckPermission(editorRole, feedbackDTO.ReceiverRole))
            {
                _logger.LogError(string.Format("User with id - {0} have no permission to edit" +
                    "feedback about {1}", claims[0].Value, nameof(feedbackDTO.ReceiverRole)));
                //what should I return
                return BadRequest();
            }

            if ((feedback.SenderId != editorId) || (feedback.SenderRole != editorRole))
            {
                _logger.LogError(string.Format("User with such id - {0} have no rights to " +
                                 "delete comment with id - {1}", claims[0].Value, feedback.ID));
                //What should I return
                return BadRequest();
            }

            if (feedback == null)
            {
                _logger.LogError(string.Format("No feedback found with such id - {0}",
                id));
                return BadRequest();
            }

            feedback.DonationId = feedbackDTO.DonationId;
            feedback.Message = feedbackDTO.Message;
            feedback.Rating = feedbackDTO.Rating;
            feedback.Time = feedbackDTO.Time;
            
            if(feedbackDTO.Image != null)
            {
                var fileName = DateTime.Now.Ticks.ToString();
                feedback.Image = _settings.Value.ServerURL + _fileUploader.CopyFileToServer(fileName,
                    nameof(DirectoryUploadName.Feedback), feedbackDTO.Image);
                _logger.LogInformation(string.Format("{0} - this path to photo was created",
                feedback.Image));
            }

            _feedbackRepository.Update(feedback);
            await _feedbackRepository.SaveChangesAsync();

            _logger.LogInformation(string.Format("Feedback with id {0} was changed", id));
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, CharityMaker, Volunteer, Representative")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 0)
            {
                _logger.LogError("Id should be bigger than -1 id = {0}", id);
                return BadRequest();
            }

            var feedback = await _feedbackRepository.GetById(id);

            var claims = HttpContext.User.Claims.ToList();
            int editorId = Convert.ToInt32(claims[2].Value);
            var editorRole = _userRoleConvertor.ConvertFromString(claims[3].Value);

            if ((editorRole != UserRole.Admin) && ((feedback.SenderId != editorId) 
                || (feedback.SenderRole != editorRole)))
            {
                _logger.LogError(string.Format("User with such id - {0} have no rights to " +
                    "delete comment with id - {1}", claims[0].Value, feedback.ID));
                //What should I return
                return BadRequest();
            }

            if (feedback == null)
            {
                _logger.LogError(string.Format("No feedback found with such id - {0}",
                 id));
                return BadRequest();
            }

            await _feedbackRepository.Delete(id);
            await _feedbackRepository.SaveChangesAsync();

            _logger.LogInformation(string.Format("Feedback with id - {0} was deleted", id));
            return Ok();
        }
    }
}