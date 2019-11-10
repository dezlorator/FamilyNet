using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DataTransferObjects;
using FamilyNet.Downloader;
using FamilyNet.Enums;
using FamilyNet.IdentityHelpers;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using FamilyNet.Models.ViewModels.AuctionLot;
using FamilyNet.StreamCreater;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyNet.Controllers
{
    public class AuctionLotController : Controller
    {
        #region Private fields

        private readonly ServerDataDownloader<AuctionLotDTO> _auctionLotDownloader;
        private readonly IURLAuctionLotBuilder _URLAuctionLotBuilder;
        private readonly string _apiAuctionLotPath = "api/v1/AuctionLot";
        private readonly IFileStreamCreater _streamCreater;

        private readonly ServerSimpleDataDownloader<DonationItemDTO> _donationItemsDownloader;
        private readonly IURLDonationItemsBuilder _URLDonationItem;
        private readonly string _apiDonationItemsPath = "api/v1/DonationItems";

        private readonly ServerDataDownloader<RepresentativeDTO> _representativesDownLoader;
        private readonly IURLRepresentativeBuilder _URLRepresentativeBuilder;
        private readonly string _apiRepresentativesPath = "api/v1/representatives";

        private readonly ServerDataDownloader<ChildDTO> _childrenDownloader;
        private readonly IURLChildrenBuilder _URLChildrenBuilder;
        private readonly string _apiChildrenPath = "api/v1/children";

        private readonly ServerChildrenHouseDownloader _childrenHouseDownloader;
        private readonly IURLChildrenHouseBuilder _URLChildrenHouseBuilder;
        private readonly string _apiChildrenHousePath = "api/v1/childrenHouse";

        private readonly IIdentityInformationExtractor _identityInformationExtactor;

        private readonly int _pageSize = 3;

        #endregion

        #region Ctor

        public AuctionLotController(ServerDataDownloader<AuctionLotDTO> auctionLotDownloader,
            IURLAuctionLotBuilder URLAuctionLotBuilder,
            IFileStreamCreater streamCreater,
            ServerSimpleDataDownloader<DonationItemDTO> donationItems,
            IURLDonationItemsBuilder URLDonationItem,
             ServerDataDownloader<RepresentativeDTO> representativesDownLoader,
             IURLRepresentativeBuilder URLRepresentativeBuilder,
             ServerDataDownloader<ChildDTO> childrenDownloader,
              IURLChildrenBuilder URLChildrenBuilder,
              IIdentityInformationExtractor identityInformationExtactor,
              ServerChildrenHouseDownloader childrenHouseDownloader,
              IURLChildrenHouseBuilder URLChildrenHouseBuilder)
        {
            _auctionLotDownloader = auctionLotDownloader;
            _URLAuctionLotBuilder = URLAuctionLotBuilder;
            _streamCreater = streamCreater;
            _donationItemsDownloader = donationItems;
            _URLDonationItem = URLDonationItem;
            _representativesDownLoader = representativesDownLoader;
            _URLRepresentativeBuilder = URLRepresentativeBuilder;
            _childrenDownloader = childrenDownloader;
            _URLChildrenBuilder = URLChildrenBuilder;
            _identityInformationExtactor = identityInformationExtactor;
            _childrenHouseDownloader = childrenHouseDownloader;
            _URLChildrenHouseBuilder = URLChildrenHouseBuilder;
        }

        #endregion

        #region ActionMethods


        public async Task<IActionResult> All(int page = 1, string name="", string sort="", string start ="", string end ="")
        {
            var url = _URLAuctionLotBuilder.SimpleQuery(_apiAuctionLotPath);
            IEnumerable<AuctionLotDTO> auctionLots = null;

            try
            {
                auctionLots = await _auctionLotDownloader.GetAllAsync(url, HttpContext.Session);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }
            catch (JsonException)
            {
                return Redirect("/Home/Error");
            }

            var crafts = auctionLots.Where(lot => lot.Status == "Approved").Select(fair => new AuctionLot()
            {
                ID = fair.ID,
                DateStart = fair.DateStart,
                Avatar = fair.PhotoParth,
                OrphanID = fair.OrphanID,
                Quantity = fair.Quantity,
                Status = fair.Status,
                AuctionLotItemID = fair.AuctionLotItemID,
                AuctionLotItem = GetItem(fair.AuctionLotItemID.Value).Result
            });

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);
            if(!Enum.TryParse<SortState>(sort, true, out var sortState))
            {
                sortState = SortState.NameAsc;
            }

            if(!float.TryParse(start, out var priceStart))
            {
                priceStart = 0;
            }
            if (!float.TryParse(end, out var priceEnd))
            {
                priceEnd = 0;
            }

            var model = new AuctionLotAllViewModel
            {
                AuctionLots = GetFiltered(crafts, priceStart, priceEnd, name, page, sortState, out var count),
                PageViewModel = new AuctionLotPageViewModel(count, page, 3),
                FilterViewModel = new AuctionLotFilterModel {  SelectedName = name,  StartPrice= start, EndPrice = end },
                Sort = sort
            };

            return View(model);
        }

        public  ActionResult MyCrafts(int orphanId = 7)
        {
            var url = _URLAuctionLotBuilder.SimpleQuery(_apiAuctionLotPath);
            IEnumerable<AuctionLotDTO> auctionLots = null;

            try
            {
                auctionLots =  _auctionLotDownloader.GetAllAsync(url, HttpContext.Session).Result.Where(lot =>lot.Status !="Approved" && lot.OrphanID == orphanId);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }
            catch (JsonException)
            {
                return Redirect("/Home/Error");
            }

            var crafts = auctionLots.Select(fair => new AuctionLot()
            {
                ID = fair.ID,
                DateStart = fair.DateStart,
                Avatar = fair.PhotoParth,
                OrphanID = fair.OrphanID,
                Quantity = fair.Quantity,
                Status = fair.Status,
                AuctionLotItemID = fair.AuctionLotItemID,
                AuctionLotItem = GetItem(fair.AuctionLotItemID.Value).Result
            });

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);

            return View(crafts);
        }

        public async Task<IActionResult> Details(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLAuctionLotBuilder.GetById(_apiAuctionLotPath, id.Value);
            AuctionLotDTO auctionLot = null;

            try
            {
                auctionLot = await _auctionLotDownloader.GetByIdAsync(url, HttpContext.Session);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }
            catch (JsonException)
            {
                return Redirect("/Home/Error");
            }

            if (auctionLot == null)
            {
                return NotFound();
            }

            var craft = new AuctionLot
            {
                ID = auctionLot.ID,
                DateStart = auctionLot.DateStart,
                Avatar = auctionLot.PhotoParth,
                OrphanID = auctionLot.OrphanID,
                Quantity = auctionLot.Quantity,
                Status = auctionLot.Status,
                AuctionLotItemID = auctionLot.AuctionLotItemID,
                AuctionLotItem = GetItem(auctionLot.AuctionLotItemID.Value).Result
            };

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);

            return View(craft);
        }

        public IActionResult Create()
        {
            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);

            return  View(new AuctionLotCreateViewModel());
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuctionLotCreateViewModel model)
        {
            model.AuctionLot.OrphanID = 7;
            var url = _URLDonationItem.CreatePost(_apiDonationItemsPath);
            var msg = await _donationItemsDownloader.CreatePostAsync(url, model.Item, HttpContext.Session);

            if (msg.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            Stream stream = null;

            if (model.AuctionLot.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(model.AuctionLot.Avatar);
            }

            var itemDTO = msg.Content.ReadAsAsync<DonationItemDTO>().Result;
            model.AuctionLot.AuctionLotItemID = itemDTO.ID;

            url = _URLAuctionLotBuilder.SimpleQuery(_apiAuctionLotPath);
            model.AuctionLot.Status = "UnApproved";
            var msg2 = await _auctionLotDownloader.CreatePostAsync(url, model.AuctionLot, stream, model.AuctionLot.Avatar.FileName, HttpContext.Session);

            if (msg2 != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);

            return Redirect("/AuctionLot/All");
        }

        // GET: Orphanages/Edit/5
    
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLAuctionLotBuilder.GetById(_apiAuctionLotPath, id.Value);
            AuctionLotDTO auctionLot = null;
            DonationItemDTO itemDTO = null;
            try
            {
                auctionLot = await _auctionLotDownloader.GetByIdAsync(url, HttpContext.Session);
                url = _URLDonationItem.GetById(_apiDonationItemsPath, auctionLot.AuctionLotItemID.Value);
                itemDTO = await _donationItemsDownloader.GetByIdAsync(url, HttpContext.Session);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }
            catch (JsonException)
            {
                return Redirect("/Home/Error");
            }



            var model = new AuctionLotCreateViewModel
            {
                AuctionLot = auctionLot,
                Item = itemDTO
            };

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);

            return View(model);
        }

        // POST: Orphanages/Edit/5
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
 
        public async Task<IActionResult> Edit(int id, AuctionLotCreateViewModel model) //TODO: Check change id position
        {
            if (id != model.AuctionLot.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Stream stream = null;

            if (model.AuctionLot.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(model.AuctionLot.Avatar);
            }

            var url = _URLDonationItem.GetById(_apiDonationItemsPath, model.AuctionLot.AuctionLotItemID.Value);

            var msg = await _donationItemsDownloader.CreatePutAsync(url, model.Item, HttpContext.Session);

            if (msg.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            url = _URLAuctionLotBuilder.GetById(_apiAuctionLotPath, id);
            model.AuctionLot.Status = "UnApproved";
            var status = await _auctionLotDownloader.CreatePutAsync(url, model.AuctionLot,
                                                            stream, model.AuctionLot.Avatar?.FileName, HttpContext.Session);

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);

            return RedirectToAction("MyCrafts");
        }


        public async Task<IActionResult> Delete(int id)
        {

            var url = _URLDonationItem.GetById(_apiAuctionLotPath, id);
            AuctionLotDTO auctionLotDTO = null;
            try
            {
                auctionLotDTO = await _auctionLotDownloader.GetByIdAsync(url, HttpContext.Session);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }

            url = _URLAuctionLotBuilder.GetById(_apiAuctionLotPath, auctionLotDTO.ID);
            var auctionStatus = await _auctionLotDownloader.DeleteAsync(url, HttpContext.Session);
            if (auctionStatus != HttpStatusCode.OK)
            {
                return Redirect("/Home/Error");
            }

            url = _URLDonationItem.GetById(_apiDonationItemsPath, auctionLotDTO.AuctionLotItemID.Value);
            var itemStatus = await _donationItemsDownloader.DeleteAsync(url, HttpContext.Session);
            if (itemStatus.StatusCode != HttpStatusCode.OK)
            {
                return Redirect("/Home/Error");
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);

            return RedirectToAction("MyCrafts");
        }


        public async Task<IActionResult> ConfirmCrafts()
        {
            int currnetReprId = 1;

            var url = _URLRepresentativeBuilder.GetById(_apiRepresentativesPath, currnetReprId);
            var representative = await _representativesDownLoader.GetByIdAsync(url, HttpContext.Session);

            url = _URLChildrenBuilder.GetAllWithFilter(_apiChildrenPath, new PersonSearchModel(), representative.ChildrenHouseID);
            var orphans = await _childrenDownloader.GetAllAsync(url, HttpContext.Session);
           

            url = _URLAuctionLotBuilder.SimpleQuery(_apiAuctionLotPath);
            IEnumerable<AuctionLotDTO> auctionLots = null;

            try
            {
                auctionLots = await _auctionLotDownloader.GetAllAsync(url, HttpContext.Session);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }
            catch (JsonException)
            {
                return Redirect("/Home/Error");
            }

            var lots = new List<IEnumerable<AuctionLot>>();
            foreach(var orphan in orphans)
            {
              var crafts = auctionLots.Where(lot => lot.Status =="UnApproved" && lot.OrphanID == orphan.ID).Select(fair => new AuctionLot()
              {
                        ID = fair.ID,
                        DateStart = fair.DateStart,
                        Avatar = fair.PhotoParth,
                        OrphanID = fair.OrphanID,
                        Quantity = fair.Quantity,
                        Status = fair.Status,
                        AuctionLotItemID = fair.AuctionLotItemID,
                        AuctionLotItem = GetItem(fair.AuctionLotItemID.Value).Result
              });
                lots.Add(crafts);
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);

            return View(lots);
        }


        public async Task<IActionResult> Confirm(int id)
        {          
            var url = _URLAuctionLotBuilder.GetById(_apiAuctionLotPath, id);
            try
            {
                var auctionLotDTO = await _auctionLotDownloader.GetByIdAsync(url, HttpContext.Session);

                auctionLotDTO.Status = "Approved";

                var msg = await _auctionLotDownloader.CreatePutAsync(url, auctionLotDTO, null, null, HttpContext.Session);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }
            catch (JsonException)
            {
                return Redirect("/Home/Error");
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);

            return Redirect("/AuctionLot/ConfirmCrafts");
        }

        public async Task<IActionResult> Decline(int id)
        {
            var url = _URLAuctionLotBuilder.GetById(_apiAuctionLotPath, id);
            try
            {
                var auctionLotDTO = await _auctionLotDownloader.GetByIdAsync(url, HttpContext.Session);

                auctionLotDTO.Status = "Declined";

                var msg = await _auctionLotDownloader.CreatePutAsync(url, auctionLotDTO, null, null, HttpContext.Session);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }
            catch (JsonException)
            {
                return Redirect("/Home/Error");
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);

            return Redirect("/AuctionLot/ConfirmCrafts");
        }


        private IEnumerable<AuctionLot> GetFiltered(IEnumerable<AuctionLot> lots, float start, float end, string name, int page,
            SortState sortOrder, out int count)
        {            
           
            if (start >0.0)
            {
                lots = lots.Where(o => o.AuctionLotItem.Price > start);
            }

            if (end > 0.0)
            {
                lots = lots.Where(o => o.AuctionLotItem.Price < end);
            }

            if (!String.IsNullOrEmpty(name))
            {
                lots = lots.Where(o => o.AuctionLotItem.Name.Contains(name));
            }

            switch (sortOrder)
            {
                case SortState.NameDesc:
                    lots = lots.OrderByDescending(s => s.AuctionLotItem.Name);
                    break;
                case SortState.PriceAsc:
                    lots = lots.OrderBy(s => s.AuctionLotItem.Price);
                    break;
                case SortState.PriceDesc:
                    lots = lots.OrderByDescending(s => s.AuctionLotItem.Price);
                    break;
            }

            count = lots.Count();

            lots = lots.Skip((page - 1) * _pageSize).Take(_pageSize);

            return lots;
        }

        #endregion

        #region Private Helpers

        private async Task<AuctionLotItem> GetItem(int id)
        {
            var urlItems = _URLDonationItem.GetById(_apiDonationItemsPath, id);
            var item = await _donationItemsDownloader.GetByIdAsync(urlItems, HttpContext.Session);

            return new AuctionLotItem
            {
                ID = item.ID,
                Name = item.Name,
                Price = item.Price,
                Description = item.Description
            };
        }

        #endregion
    }
}