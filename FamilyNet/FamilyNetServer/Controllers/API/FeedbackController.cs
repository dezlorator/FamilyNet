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
using NLog;
using DataTransferObjects;
using Microsoft.Extensions.Options;
using FamilyNetServer.Configuration;
using FamilyNetServer.Models.Identity;
using FamilyNetServer.Uploaders;
using FamilyNetServer.Enums;
using Microsoft.AspNetCore.Authorization;

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
        #endregion

        public FeedbackController(EFRepository<Feedback> feedbackRepository,
            ILogger<FeedbackController> logger, IFeedbackValidator validator,
            IOptionsSnapshot<ServerURLSettings> settings,
            IFileUploader fileUploader)
        {
            _feedbackRepository = feedbackRepository;
            _logger = logger;
            _validator = validator;
            _settings = settings;
            _fileUploader = fileUploader;
        }

        [HttpGet("donation/{donationId}")]
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

            string photoPath = string.Empty;

            if(feedbackDTO.Image != null)
            {
                var fileName = feedbackDTO.Time.ToString();

                photoPath = _settings.Value.ServerURL + _fileUploader.CopyFileToServer(fileName,
                    nameof(DirectoryUploadName.Feedback), feedbackDTO.Image);
                _logger.LogInformation(string.Format("{0} - this path to photo was created", photoPath));
            }

            var feedback = new Feedback()
            {
                DonationId = feedbackDTO.DonationId,
                Image = photoPath,
                Message = feedbackDTO.Message,
                Rating = feedbackDTO.Rating,
                ReceiverId = feedbackDTO.ReceiverId,
                ReceiverRole = feedbackDTO.ReceiverRole,
                Time = feedbackDTO.Time,
                SenderId = feedbackDTO.SenderId,
                SenderRole = feedbackDTO.SenderRole
            };

            await _feedbackRepository.Create(feedback);
            await _feedbackRepository.SaveChangesAsync();

            _logger.LogInformation(string.Format("Feedback was created, date{0}", feedbackDTO.Time));
            return Created("api/v1/charityMakers/" + feedbackDTO.DonationId, feedback);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Edit(int id, FeedbackDTO feedbackDTO)
        {
            if(id < 0)
            {
                _logger.LogError("Id should be bigger than -1 id = {0}", id);
                return BadRequest();
            }

            var feedback = await _feedbackRepository.GetById(id);

            if(feedback == null)
            {
                _logger.LogError(string.Format("No feedback found with such id - {0}",
                id));
                return BadRequest();
            }

            feedback.DonationId = feedbackDTO.DonationId;
            feedback.Message = feedbackDTO.Message;
            feedback.Rating = feedbackDTO.Rating;
            feedback.ReceiverId = feedbackDTO.ReceiverId;
            feedback.ReceiverRole = feedbackDTO.ReceiverRole;
            feedback.SenderId = feedbackDTO.SenderId;
            feedback.SenderRole = feedbackDTO.SenderRole;
            feedback.Time = feedbackDTO.Time;
            
            if(feedbackDTO.Image != null)
            {
                var fileName = feedbackDTO.Time.ToString();
                feedback.Image = _fileUploader.CopyFileToServer(fileName,
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
        [Authorize(Roles = "Admin")]
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

            if(feedback == null)
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