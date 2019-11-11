using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;
using DataTransferObjects.Enums;
using FamilyNet.Downloader;
using FamilyNet.Models;
using FamilyNet.StreamCreater;
using Microsoft.AspNetCore.Mvc;

namespace FamilyNet.Controllers
{
    public class FeedbackController : Controller
    {
        #region private
        private readonly IURLFeedbackBuilder _urlFeedbackBuilder;
        private readonly ServerDataDownloader<FeedbackDTO> _feedbackDownloader;
        private const string _feedbackApiPath = "api/v1/feedback";
        private readonly IFileStreamCreater _streamCreator;
        private readonly string _pathToErrorView = "/Home/Error";
        #endregion

        #region ctor
        public FeedbackController(IURLFeedbackBuilder urlFeedbackBuilder,
            ServerDataDownloader<FeedbackDTO> feedbackDownloader)
        {
            _urlFeedbackBuilder = urlFeedbackBuilder;
            _feedbackDownloader = feedbackDownloader;
        }
        #endregion
    }
}