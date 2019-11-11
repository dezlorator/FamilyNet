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
        private readonly string _fioApiPath = "api/v1/fio";
        private readonly IFileStreamCreater _streamCreator;
        private readonly string _pathToErrorView = "/Home/Error";
        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        private readonly IURLDonationsBuilder _urlDonationsBuilder;
        private readonly ServerSimpleDataDownloader<DonationDetailDTO> _donationDownloader;
        private readonly string _donationApiPath = "api/v1/donations";
        private readonly ServerChildrenHouseDownloader _childrenHouseDownloader;
        private readonly IURLChildrenHouseBuilder _URLChildrenHouseBuilder;
        private readonly string _houseApiPath = "api/v1/childrenHouse";
        private int _donationId;
        #endregion

        #region ctor
        public FeedbackController(IURLFeedbackBuilder urlFeedbackBuilder,
            ServerDataDownloader<FeedbackDTO> feedbackDownloader,
            IFileStreamCreater streamCreator, IFioDownloader fioDownloader,
            IURLFioBuilder urlFioBuilder,
            IIdentityInformationExtractor identityInformationExtactor,
            IURLDonationsBuilder urlDonationsBuilder, 
            ServerSimpleDataDownloader<DonationDetailDTO> donationDownloader)
        {
            _urlFeedbackBuilder = urlFeedbackBuilder;
            _feedbackDownloader = feedbackDownloader;
            _urlFioBuilder = urlFioBuilder;
            _fioDownloader = fioDownloader;
            _streamCreator = streamCreator;
            _identityInformationExtactor = identityInformationExtactor;
            _urlDonationsBuilder = urlDonationsBuilder;
            _donationDownloader = donationDownloader;
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

            var viewModel = new CreateFeedbackViewModel()
            {
                feedbackDTO = new FeedbackDTO(),
                Roles = GetAllowedReceiversByRole(role)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFeedbackViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            Stream stream = null;
            if (viewModel.feedbackDTO.Image != null)
            {
                stream = _streamCreator.CopyFileToStream(viewModel.feedbackDTO.Image);
            }

            var feedbackUrl = _urlFeedbackBuilder.CreatePost(_feedbackApiPath);
            var status = await _feedbackDownloader.CreatePostAsync(feedbackUrl, viewModel.feedbackDTO,
                                                             stream, viewModel.feedbackDTO.Image.FileName,
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

        private List<string> GetAllowedReceiversByRole(string role)
        {
            switch(role)
            {
                case "CharityMaker":
                    return new List<string>()
                    {
                        "Volunteer"
                    };
                case "Representative":
                    return new List<string>()
                    {
                        "Volunteer"
                    };
                case "Volunteer":
                    return new List<string>()
                    {
                        "Reprecentative",
                        "CharityMaker"
                    };
                default:
                    return new List<string>();
            }
        }

        private async Task<FeedbackDTO> GetReceiverIdByRole(string role, int donationId, FeedbackDTO feedback)
        {
            var url = _urlDonationsBuilder.GetById(_donationApiPath, donationId);

            var donation = _donationDownloader.GetByIdAsync(url, HttpContext.Session);
            UserRole receiverRole;
            int receiverId;

            switch(role)
            {
                case "CharityMaker":
                    receiverRole = UserRole.CharityMaker;
                    receiverId = donation.Result.CharityMakerID??0;
                    break;
                case "Representative":
                    receiverRole = UserRole.Representative;
                    var houseUrl = _URLChildrenHouseBuilder.GetAllWithFilter(_houseApiPath, null, SortStateOrphanages.AddressAsc);
                    var houses = _childrenHouseDownloader.GetAllAsync(houseUrl, HttpContext.Session);
                    
                    break;
                case "Volunteer":
                    receiverRole = UserRole.Volunteer;

                    //смерджусь тогда можно будет
                    break;
                default:
                    return feedback;

            }

            feedback.ReceiverRole = receiverRole;

            return feedback;
            
        }

    }
}

