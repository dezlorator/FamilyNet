using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DataTransferObjects;
using FamilyNet.Downloader;
using FamilyNet.Enums;
using FamilyNet.IdentityHelpers;
using FamilyNet.Models.ViewModels;
using FamilyNet.Models.ViewModels.Purchase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyNet.Controllers
{
    public class PurchaseController : Controller
    {
        #region Private fields

        private readonly ServerDataDownloader<AuctionLotDTO> _auctionLotDownloader;
        private readonly IURLAuctionLotBuilder _URLAuctionLotBuilder;
        private readonly string _apiAuctionLotPath = "api/v1/AuctionLot";

        private readonly ServerSimpleDataDownloader<DonationItemDTO> _donationItemsDownloader;
        private readonly IURLDonationItemsBuilder _URLDonationItem;
        private readonly string _apiDonationItemsPath = "api/v1/DonationItems";

        private readonly ServerSimpleDataDownloader<PurchaseDTO> _purchaseDownloader;
        private readonly IURLPurchaseBuilder _URLPurchase;
        private readonly string _apiPurchasePath = "api/v1/Purchase";

        private readonly IIdentityInformationExtractor _identityInformationExtactor;

        private readonly int _pageSize = 3;

        #endregion

        #region Ctor

        public PurchaseController(ServerDataDownloader<AuctionLotDTO> auctionLotDownloader,
            IURLAuctionLotBuilder URLAuctionLotBuilder,
            ServerSimpleDataDownloader<DonationItemDTO> donationItems,
            IURLDonationItemsBuilder URLDonationItem,
              IIdentityInformationExtractor identityInformationExtactor,
            ServerSimpleDataDownloader<PurchaseDTO> purchaseDownloader,
             IURLPurchaseBuilder URLPurchase)
        {
            _auctionLotDownloader = auctionLotDownloader;
            _URLAuctionLotBuilder = URLAuctionLotBuilder;
            _donationItemsDownloader = donationItems;
            _URLDonationItem = URLDonationItem;
            _identityInformationExtactor = identityInformationExtactor;
            _purchaseDownloader = purchaseDownloader;
            _URLPurchase = URLPurchase;
        }

        #endregion


        public async Task<IActionResult> Buy(int id)
        {
            var userId = HttpContext.Session.GetString("id");

            var purchase = new PurchaseDTO
            {
               UserId = userId,
               AuctionLotId = id
            };


            var url = _URLAuctionLotBuilder.GetById(_apiAuctionLotPath, id);
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

            var urlItems = _URLDonationItem.GetById(_apiDonationItemsPath, auctionLot.AuctionLotItemID.Value);
            var item = await _donationItemsDownloader.GetByIdAsync(urlItems, HttpContext.Session);

            if (item == null)
            {
                return NotFound();
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                ViewData);

            var viewModel = new PurchaseBuyViewModel
            {
                Purchase = purchase,
                AuctionLot = auctionLot,
                Item = item
            };

            return View(viewModel);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult Confirm(PurchaseBuyViewModel model)
        {
            if(model.Purchase.Quantity > model.AuctionLot.Quantity)
            {
                _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                       ViewData);

                return View("TooMuch");
            }
            model.Purchase.Paid = model.Item.Price * model.Purchase.Quantity;

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                       ViewData);

            return View(model);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> BuyConfirmed(PurchaseBuyViewModel model)
        {
            var url = _URLPurchase.SimpleQuery(_apiPurchasePath);

            model.Purchase.Date = DateTime.Now;
            var msg = await _purchaseDownloader.CreatePostAsync(url, model.Purchase, HttpContext.Session);

            if (msg.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            url = _URLAuctionLotBuilder.GetById(_apiAuctionLotPath, model.Purchase.AuctionLotId);
            var lot = await _auctionLotDownloader.GetByIdAsync(url, HttpContext.Session);

            lot.Quantity -= model.Purchase.Quantity;
            if(lot.Quantity == 0)
            {
                lot.Status = "Sold";
            }
            
            var status = await _auctionLotDownloader.CreatePutAsync(url, lot, null, String.Empty, HttpContext.Session);

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                         ViewData);

            return Redirect("/AuctionLot/All");
        }

        public async Task<IActionResult> All(int mypage = 1,
            string email = "", string sort = "",
            string craft = "", string date = "")
        {
            string currentUserMail = null;

            if (HttpContext.Session.GetString("roles") != "Admin")
            {
                currentUserMail = HttpContext.Session.GetString("email");
            }


            if (!DateTime.TryParse(date, out var dateTime))
            {
                dateTime = DateTime.MinValue;
            }

            var url = _URLPurchase.GetAllFiltered(_apiPurchasePath, new FilterParamentrsPurchaseDTO
            {
                CraftName = craft,
                Date = dateTime,
                Page =mypage,                
                Rows = _pageSize,
                Email = currentUserMail !=null? currentUserMail: email,
                Sort = sort
            });

            IEnumerable<PurchaseDTO> purchases = null;

            try
            {
                if (HttpContext.Session.GetString("roles") == "Admin")
                {
                    purchases = await _purchaseDownloader.GetAllAsync(url, HttpContext.Session);
                }
                else
                {
                    purchases = await _purchaseDownloader.GetAllAsync(url, HttpContext.Session);
                }
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

            if (purchases == null)
            {
                return NotFound();
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                ViewData);

            var filter = new PurchaseFilterViewModel
            {
                Email = email,
                CraftName = craft,
                Date = dateTime,
            };

            var viewModel = new PurchaseAllViewModel
            {
                PurchaseDTO = purchases,
                FilterViewModel = filter,
                PageViewModel = new PageViewModel(_purchaseDownloader.TotalItemsCount, mypage, _pageSize),
                Sort = sort
            };


            return View(viewModel);
        }

      
    }
}