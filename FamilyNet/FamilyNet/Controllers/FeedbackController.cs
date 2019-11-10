using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DataTransferObjects;
using DataTransferObjects.Enums;
using FamilyNet.Downloader;
using FamilyNet.Downloader.Interfaces;
using FamilyNet.Encoders;
using FamilyNet.IdentityHelpers;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using FamilyNet.StreamCreater;
using Microsoft.AspNetCore.Http;
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
        private int _donationId;
        private readonly IJWTEncoder _encoder;
        private readonly IAuthorizeCreater _authorizeCreater;
        #endregion

        #region ctor
        public FeedbackController(IURLFeedbackBuilder urlFeedbackBuilder,
            ServerDataDownloader<FeedbackDTO> feedbackDownloader,
            IFileStreamCreater streamCreator, IFioDownloader fioDownloader,
            IURLFioBuilder urlFioBuilder,
            IIdentityInformationExtractor identityInformationExtactor,
            IJWTEncoder encoder, IAuthorizeCreater authorizeCreater)
        {
            _urlFeedbackBuilder = urlFeedbackBuilder;
            _feedbackDownloader = feedbackDownloader;
            _urlFioBuilder = urlFioBuilder;
            _fioDownloader = fioDownloader;
            _streamCreator = streamCreator;
            _identityInformationExtactor = identityInformationExtactor;
            _encoder = encoder;
            _authorizeCreater = authorizeCreater;
        }
        #endregion

        public async Task<IActionResult> FeedbackByDonationId(int donationId)
        {
            _donationId = 1;
            donationId = 1;

            var feedbackUrl = _urlFeedbackBuilder.GetByDonationId(_feedbackApiPath, donationId);

            IEnumerable<FeedbackDTO> feedbackContainer = null;

            try
            {
                feedbackContainer = await _feedbackDownloader.GetAllAsync(feedbackUrl, HttpContext.Session);
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

            List<FeedbackVIewModel> feedbackViewModelContainer = new List<FeedbackVIewModel>();

            foreach(var item in feedbackContainer)
            {
                FeedbackVIewModel feedback = new FeedbackVIewModel()
                {
                    FeedbackDTO = item,
                    SenderFio = GetFio(item.SenderId, item.SenderRole).Result,
                    ReceiverFio = GetFio(item.ReceiverId??0, item.ReceiverRole).Result
                };
                feedbackViewModelContainer.Add(feedback);
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return View(feedbackViewModelContainer);
        }

        public async Task<IActionResult> Details(int id)
        {
            var feedbackUrl = _urlFeedbackBuilder.GetById(_feedbackApiPath, id);

            FeedbackDTO feedback = null;

            try
            {
                feedback = await _feedbackDownloader.GetByIdAsync(feedbackUrl, HttpContext.Session);
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

            var feedbackViewModel = new FeedbackVIewModel()
            {
                FeedbackDTO = feedback,
                ReceiverFio = GetFio(feedback.ReceiverId ?? 0, feedback.ReceiverRole).Result,
                SenderFio = GetFio(feedback.SenderId, feedback.SenderRole).Result
            };

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                ViewData);

            return View(feedbackViewModel);
        }

        public async Task<IActionResult> Create()
        {
            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);
            var role = HttpContext.Session.GetString("roles");
            var personId = HttpContext.Session.GetString("personId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedbackDTO feedbackDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(feedbackDTO);
            }
            Stream stream = null;
            if (feedbackDTO.Image != null)
            {
                stream = _streamCreator.CopyFileToStream(feedbackDTO.Image);
            }

            var feedbackUrl = _urlFeedbackBuilder.CreatePost(_feedbackApiPath);
            var status = await _feedbackDownloader.CreatePostAsync(feedbackUrl, feedbackDTO,
                                                             stream, feedbackDTO.Image.FileName,
                                                             HttpContext.Session);

            if (status == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (status != HttpStatusCode.Created)
            {
                return Redirect(_pathToErrorView);
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return Redirect("/feedback/FeedbackByDonationId");
        }

        private async Task<FioDTO> GetFio(int id, UserRole role)
        {
            var url = _urlFioBuilder.GetById(_fioApiPath, id, role);
            var fio = await _fioDownloader.GetByIdAsync(url, HttpContext.Session);
            fio.Role = role.ToString();
            return fio;
        }
    }
}

