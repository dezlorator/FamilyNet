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

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        #region private
        private EFRepository<Feedback> _feedbackRepository;
        private ILogger<FeedbackController> _logger;
        private IFeedbackValidator _validator;
        private readonly IOptionsSnapshot<ServerURLSettings> _settings;
        #endregion

        public FeedbackController(EFRepository<Feedback> feedbackRepository,
            ILogger<FeedbackController> logger, IFeedbackValidator validator,
            IOptionsSnapshot<ServerURLSettings> settings)
        {
            _feedbackRepository = feedbackRepository;
            _logger = logger;
            _validator = validator;
            _settings = settings;
        }

        [HttpGet("{donationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetByDonateId(int donationId)
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

            foreach(var feedback in feedbackContainer)
            {
                feedbackDTO.Add(new FeedbackDTO
                {
                    DonationId = feedback.DonationId,
                    ImagePath = feedback.Image,
                    Message = feedback.Message,
                    Rating = feedback.Rating,
                    ReceiverId = feedback.ReceiverId,
                    ReceiverRole = feedback.ReceiverRole,
                    Time = feedback.Time,
                    SenderId = feedback.SenderId?? 0,
                    SenderRole = feedback.SenderRole
                });
            }

            return Ok(feedbackDTO);
        }
    }
}