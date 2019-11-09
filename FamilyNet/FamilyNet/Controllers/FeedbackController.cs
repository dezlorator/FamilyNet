using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataTransferObjects;
using DataTransferObjects.Enums;
using FamilyNet.Downloader;
using FamilyNet.Downloader.Interfaces;
using FamilyNet.IdentityHelpers;
using FamilyNet.Models;
using FamilyNet.StreamCreater;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyNet.Controllers
{
    public class FeedbackController : Controller
    {
        #region private
        private readonly IURLFeedbackBuilder _urlFeedbackBuilder;
        private readonly ServerDataDownloader<FeedbackDTO> _feedbackDownloader;
        private const string _feedbackApiPath = "api/v1/feedback";
        private readonly IURLFioBuilder _urlFioBuilder;
        private readonly IFioDownloader _fioDownloader;
        private const string _fioApiPath = "api/v1/fio";
        private readonly IFileStreamCreater _streamCreator;
        private readonly string _pathToErrorView = "/Home/Error";
        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        #endregion

        #region ctor
        public FeedbackController(IURLFeedbackBuilder urlFeedbackBuilder,
            ServerDataDownloader<FeedbackDTO> feedbackDownloader,
            IFileStreamCreater streamCreator, IFioDownloader fioDownloader,
            IURLFioBuilder urlFioBuilder,
            IIdentityInformationExtractor identityInformationExtactor)
        {
            _urlFeedbackBuilder = urlFeedbackBuilder;
            _feedbackDownloader = feedbackDownloader;
            _urlFioBuilder = urlFioBuilder;
            _fioDownloader = fioDownloader;
            _streamCreator = streamCreator;
            _identityInformationExtactor = identityInformationExtactor;
        }
        #endregion

        public async Task<IActionResult> FeedbackByDonationId(int donationId)
        {
            var url = _urlFeedbackBuilder.GetByDonationId(_feedbackApiPath, donationId);

            IEnumerable<FeedbackDTO> feedbackContainer = null;

            try
            {
                feedbackContainer = await _feedbackDownloader.GetAllAsync(url, HttpContext.Session);
            }
            catch (ArgumentNullException)
            {
                return Redirect(_pathToErrorView);
            }
            catch (HttpRequestException)
            {
                return Redirect(_pathToErrorView);
            }
            catch (JsonException)
            {
                return Redirect(_pathToErrorView);
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return View(feedbackContainer);
        }
    }
}