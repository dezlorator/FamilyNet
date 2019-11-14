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
using FamilyNet.IdentityHelpers;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using FamilyNet.StreamCreater;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FamilyNet.Downloader.Interfaces;

namespace FamilyNet.Controllers
{
    public class FeedbackController : Controller
    {
        #region private

        private readonly IURLFeedbackBuilder _urlFeedbackBuilder;
        private readonly IURLFioBuilder _urlFioBuilder;
        private readonly IURLDonationsBuilder _urlDonationsBuilder;
        private readonly IURLRepresentativeBuilder _urlRepresentativeBuilder;
        private readonly IURLQuestsBuilder _urlQuestsBuilder;

        private readonly ServerDataDownloader<FeedbackDTO> _feedbackDownloader;
        private readonly ServerSimpleDataDownloader<DonationDetailDTO> _donationDownloader;
        private readonly ServerSimpleDataDownloader<QuestDTO> _questsDownloader;
        private readonly IServerRepresenativesDataDownloader _representativeDataDownloader;
        private readonly IFioDownloader _fioDownloader;

        private const string _snpApiPath = "api/v1/sNP";
        private const string _feedbackApiPath = "api/v1/feedback";
        private const string _pathToErrorView = "/Home/Error";
        private const string _donationApiPath = "api/v1/donations";
        private const string _representativeApiPath = "api/v1/representatives";
        private const string _questApiPath = "api/v1/quests";

        private readonly IFileStreamCreater _streamCreator;
        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        private static int _donationId;
        #endregion

        #region ctor
        public FeedbackController(IURLFeedbackBuilder urlFeedbackBuilder,
            ServerDataDownloader<FeedbackDTO> feedbackDownloader,
            IFileStreamCreater streamCreator, IFioDownloader fioDownloader,
            IURLFioBuilder urlFioBuilder,
            IIdentityInformationExtractor identityInformationExtactor,
            IURLDonationsBuilder urlDonationsBuilder,
            ServerSimpleDataDownloader<DonationDetailDTO> donationDownloader,
            IURLRepresentativeBuilder urlRepresentativeBuilder,
            IServerRepresenativesDataDownloader representativeDataDownloader,
            IURLQuestsBuilder urlQuestsBuilder, ServerSimpleDataDownloader<QuestDTO> questsDownloader)
        {
            _urlFeedbackBuilder = urlFeedbackBuilder;
            _feedbackDownloader = feedbackDownloader;
            _urlFioBuilder = urlFioBuilder;
            _fioDownloader = fioDownloader;
            _streamCreator = streamCreator;
            _identityInformationExtactor = identityInformationExtactor;
            _urlDonationsBuilder = urlDonationsBuilder;
            _donationDownloader = donationDownloader;
            _urlRepresentativeBuilder = urlRepresentativeBuilder;
            _representativeDataDownloader = representativeDataDownloader;
            _urlQuestsBuilder = urlQuestsBuilder;
            _questsDownloader = questsDownloader;
        }
        #endregion

        public async Task<IActionResult> FeedbackByDonationId(int id)
        {
            if(id != 0)
            {
                _donationId = id;
            }

            var feedbackUrl = _urlFeedbackBuilder.GetByDonationId(_feedbackApiPath, _donationId);

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

            if (feedbackContainer != null)
            {
                foreach (var item in feedbackContainer)
                {
                    FeedbackVIewModel feedback = new FeedbackVIewModel()
                    {
                        FeedbackDTO = item,
                        SenderFio = GetFio(item.SenderId, item.SenderRole).Result,
                        ReceiverFio = GetFio(item.ReceiverId ?? 0, item.ReceiverRole).Result
                    };
                    feedbackViewModelContainer.Add(feedback);
                }
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
            viewModel.feedbackDTO.Image = viewModel.Avatar;
            Stream stream = null;
            if (viewModel.feedbackDTO.Image != null)
            {
                stream = _streamCreator.CopyFileToStream(viewModel.feedbackDTO.Image);
            }

            viewModel.feedbackDTO.DonationId = _donationId;
            viewModel.feedbackDTO = await GetReceiverInfoByRole(viewModel.Roles[0], viewModel.feedbackDTO.DonationId,
                viewModel.feedbackDTO);
            viewModel.feedbackDTO.Time = DateTime.Now;

            var feedbackUrl = _urlFeedbackBuilder.CreatePost(_feedbackApiPath);
            var status = await _feedbackDownloader.CreatePostAsync(feedbackUrl, viewModel.feedbackDTO,
                                                             stream, viewModel.feedbackDTO.Image?.FileName,
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

        public async Task<IActionResult> Edit(int id)
        {
            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                ViewData);

            var feedbackUrl = _urlFeedbackBuilder.GetById(_feedbackApiPath, id);
            var feedback = await _feedbackDownloader.GetByIdAsync(feedbackUrl, HttpContext.Session);

            var role = HttpContext.Session.GetString("roles");

            var viewModel = new CreateFeedbackViewModel()
            {
                feedbackDTO = feedback,
                Roles = GetAllowedReceiversByRole(role)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateFeedbackViewModel viewModel)
        {
            if (id != viewModel.feedbackDTO.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            Stream stream = null;

            if (viewModel.feedbackDTO.Image != null)
            {
                stream = _streamCreator.CopyFileToStream(viewModel.feedbackDTO.Image);
            }

            viewModel.feedbackDTO.DonationId = _donationId;
            viewModel.feedbackDTO = await GetReceiverInfoByRole(viewModel.Roles[0], viewModel.feedbackDTO.DonationId,
                viewModel.feedbackDTO);
            var url = _urlFeedbackBuilder.GetById(_feedbackApiPath, id);
            var status = await _feedbackDownloader.CreatePutAsync(url, viewModel.feedbackDTO,
                                                            stream, viewModel.feedbackDTO.Image?.FileName,
                                                            HttpContext.Session);

            if (status == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (status == HttpStatusCode.Forbidden)
            {
                return Redirect(_pathToErrorView);
            }

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect(_pathToErrorView);
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return Redirect("/feedback/FeedbackByDonationId");
        }

        public async Task<IActionResult> Delete(int id)
        {
            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                    ViewData);

            var feedbackUrl = _urlFeedbackBuilder.GetById(_feedbackApiPath, id);
            var feedback = await _feedbackDownloader.GetByIdAsync(feedbackUrl, HttpContext.Session);

            return View(feedback);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var url = _urlFeedbackBuilder.GetById(_feedbackApiPath, id);
            var status = await _feedbackDownloader.DeleteAsync(url, HttpContext.Session);

            if (status == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (status == HttpStatusCode.Forbidden)
            {
                return Redirect(_pathToErrorView);
            }

            if (status != HttpStatusCode.OK)
            {
                return Redirect(_pathToErrorView);
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return Redirect("/feedback/FeedbackByDonationId");
        }

        private async Task<SNPDTO> GetFio(int id, UserRole role)
        {
            var url = _urlFioBuilder.GetById(_snpApiPath, id, role);
            var fio = await _fioDownloader.GetByIdAsync(url, HttpContext.Session);
            fio.Role = role.ToString();

            return fio;
        }

        private List<string> GetAllowedReceiversByRole(string role)
        {
            switch (role)
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
                        "Representative",
                        "CharityMaker"
                    };
                default:
                    return new List<string>();
            }
        }

        private async Task<FeedbackDTO> GetReceiverInfoByRole(string role, int donationId, FeedbackDTO feedback)
        {
            var url = _urlDonationsBuilder.GetById(_donationApiPath, donationId);

            var donation = await _donationDownloader.GetByIdAsync(url, HttpContext.Session);
            UserRole receiverRole;
            int receiverId = 0;

            switch (role)
            {
                case "CharityMaker":
                    receiverRole = UserRole.CharityMaker;
                    receiverId = donation.CharityMakerID ?? 0;
                    break;
                case "Representative":
                    receiverRole = UserRole.Representative;
                    var representativesURL = _urlRepresentativeBuilder.GetByChildrenHouseId(_representativeApiPath,
                        donation.OrphanageID ?? 0);
                    var representatives = await _representativeDataDownloader.GetByChildrenHouseIdAsync
                        (representativesURL, HttpContext.Session);
                    receiverId = representatives[0].ID;
                    break;
                case "Volunteer":
                    receiverRole = UserRole.Volunteer;
                    var questUrl = _urlQuestsBuilder.GetAllWithFilter(_questApiPath, null, null);
                    var quest = await _questsDownloader.GetAllAsync(questUrl, HttpContext.Session);
                    receiverId = quest.Where(p => p.DonationID == donationId).ToList()[0].VolunteerID??0;
                    break;
                default:
                    return feedback;

            }

            feedback.ReceiverRole = receiverRole;
            feedback.ReceiverId = receiverId;

            return feedback;

        }
    }
}