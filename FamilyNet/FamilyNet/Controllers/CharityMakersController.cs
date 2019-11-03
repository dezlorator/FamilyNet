using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using FamilyNet.Downloader;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using FamilyNet.StreamCreater;
using DataTransferObjects;
using FamilyNet.IdentityHelpers;

namespace FamilyNet.Controllers
{
    public class CharityMakersController : Controller
    {
        #region private

        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        private readonly IURLCharityMakerBuilder _urlBilder;
        private readonly ServerDataDownloader<CharityMakerDTO> _serverDownloader;
        private readonly string _apiPath = "api/v1/charityMakers";
        private readonly IFileStreamCreater _streamCreator;
        private readonly string _pathToErrorView = "/Home/Error";
        private readonly string _pathToAdressApi = "api/v1/address";
        private readonly IURLAddressBuilder _urlAdressBuilder;
        private readonly IServerAddressDownloader _serverAddressDownloader;

        #endregion

        #region ctor

        public CharityMakersController(IURLCharityMakerBuilder urlCharityMakerBuilder,
                ServerDataDownloader<CharityMakerDTO> downloader,
                IFileStreamCreater streamCreator,
                IURLAddressBuilder urlAdressBuilder,
                IServerAddressDownloader addressDownloader,
                IIdentityInformationExtractor identityInformationExtactor)
        {
            _urlBilder = urlCharityMakerBuilder;
            _serverDownloader = downloader;
            _streamCreator = streamCreator;
            _urlAdressBuilder = urlAdressBuilder;
            _serverAddressDownloader = addressDownloader;
            _identityInformationExtactor = identityInformationExtactor;
        }

        #endregion

        public async Task<IActionResult> Index(int id, PersonSearchModel searchModel)
        {
            var url = _urlBilder.GetAllWithFilter(_apiPath,
                                                   searchModel,
                                                   id);
            IEnumerable<CharityMakerDTO> charityMakerContainer = null;

            try
            {
                charityMakerContainer = await _serverDownloader.GetAllAsync(url, HttpContext.Session);
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

            var charityMaker = charityMakerContainer.Select(charMaker => new CharityMaker()
            {
                Birthday = charMaker.Birthday,
                FullName = new FullName()
                {
                    Name = charMaker.Name,
                    Patronymic = charMaker.Patronymic,
                    Surname = charMaker.Surname
                },
                ID = charMaker.ID,
                Avatar = charMaker.PhotoPath,
                AddressID = charMaker.AdressID,
                EmailID = charMaker.EmailID,
                Rating = charMaker.Rating
            });

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return View(charityMaker);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _urlBilder.GetById(_apiPath, id.Value);
            CharityMakerDTO charityMakerDTO = null;

            try
            {
                charityMakerDTO = await _serverDownloader.GetByIdAsync(url, HttpContext.Session);

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

            var addressUrl = _urlAdressBuilder.GetById(_pathToAdressApi, charityMakerDTO.AdressID);
            AddressDTO adderessDTO = null;

            try
            {
                adderessDTO = await _serverAddressDownloader.GetByIdAsync(addressUrl, HttpContext.Session);

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

            if (charityMakerDTO == null)
            {
                return NotFound();
            }

            var charityMaker = new CharityMaker()
            {
                Birthday = charityMakerDTO.Birthday,
                FullName = new FullName()
                {
                    Name = charityMakerDTO.Name,
                    Patronymic = charityMakerDTO.Patronymic,
                    Surname = charityMakerDTO.Surname
                },
                ID = charityMakerDTO.ID,
                Avatar = charityMakerDTO.PhotoPath,
                Address = new Address()
                {
                    Country = adderessDTO.Country,
                    Region = adderessDTO.Region,
                    City = adderessDTO.City,
                    Street = adderessDTO.Street,
                    House = adderessDTO.House
                },
                EmailID = charityMakerDTO.EmailID,
                Rating = charityMakerDTO.Rating,
            };

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return View(charityMaker);
        }


        public async Task<IActionResult> Create()
        {
            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);
            return View();
        }

        // POST: CharityMakers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CharityMakerDTO charityMakerDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(charityMakerDTO);
            }
            Stream stream = null;
            if (charityMakerDTO.Avatar != null)
            {
                stream = _streamCreator.CopyFileToStream(charityMakerDTO.Avatar);
            }

            var addressUrl = _urlAdressBuilder.CreatePost(_pathToAdressApi);
            var status1 = await _serverAddressDownloader.CreatePostAsync(addressUrl,
                                                        charityMakerDTO.AddressDTO,
                                                        HttpContext.Session);

            charityMakerDTO.AdressID = status1.Content.ReadAsAsync<AddressDTO>().Result.ID;
            var url = _urlBilder.CreatePost(_apiPath);
            var status = await _serverDownloader.CreatePostAsync(url, charityMakerDTO,
                                                             stream, charityMakerDTO.Avatar.FileName,
                                                             HttpContext.Session);
            if (status == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (status != HttpStatusCode.Created)
            {
                return Redirect(_pathToErrorView);
                //TODO: log
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return Redirect("/charityMakers/Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _urlBilder.GetById(_apiPath, id.Value);
            CharityMakerDTO charityMakerDTO = null;

            try
            {
                charityMakerDTO = await _serverDownloader.GetByIdAsync(url, HttpContext.Session);
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

            var adressUrl = _urlAdressBuilder.GetById(_pathToAdressApi, charityMakerDTO.AdressID);
            AddressDTO addressDTO = null;

            try
            {
                addressDTO = await _serverAddressDownloader.GetByIdAsync(adressUrl, HttpContext.Session);
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

            charityMakerDTO.AddressDTO = new AddressDTO();
            charityMakerDTO.AddressDTO.ID = addressDTO.ID;
            charityMakerDTO.AddressDTO.Country = addressDTO.Country;
            charityMakerDTO.AddressDTO.Region = addressDTO.Region;
            charityMakerDTO.AddressDTO.City = addressDTO.City;
            charityMakerDTO.AddressDTO.Street = addressDTO.Street;
            charityMakerDTO.AddressDTO.House = addressDTO.House;

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return View(charityMakerDTO);
        }

        // POST: CharityMakers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CharityMakerDTO charityMakerDTO)
        {
            if (id != charityMakerDTO.ID)
            {
                return NotFound();
            }
            charityMakerDTO.AddressDTO.ID = charityMakerDTO.AdressID;
            if (!ModelState.IsValid)
            {
                return View(charityMakerDTO);
            }

            Stream stream = null;

            if (charityMakerDTO.Avatar != null)
            {
                stream = _streamCreator.CopyFileToStream(charityMakerDTO.Avatar);
            }

            var url = _urlBilder.GetById(_apiPath, id);
            var status = await _serverDownloader.CreatePutAsync(url, charityMakerDTO,
                                                            stream, charityMakerDTO.Avatar?.FileName,
                                                            HttpContext.Session);

            var addressUrl = _urlAdressBuilder.GetById(_pathToAdressApi, charityMakerDTO.AdressID);
            var status1 = await _serverAddressDownloader.CreatePutAsync(addressUrl,
                                                            charityMakerDTO.AddressDTO, HttpContext.Session);

            if (status == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect(_pathToErrorView);
                //TODO: log
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return Redirect("/charityMakers/Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _urlBilder.GetById(_apiPath, id.Value);
            CharityMakerDTO charityMakerDTO = null;

            try
            {
                charityMakerDTO = await _serverDownloader.GetByIdAsync(url, HttpContext.Session);
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

            if (charityMakerDTO == null)
            {
                return NotFound();
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return View(charityMakerDTO);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var url = _urlBilder.GetById(_apiPath, id);
            var status = await _serverDownloader.DeleteAsync(url, HttpContext.Session);

            if (status == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (status != HttpStatusCode.OK)
            {
                return Redirect(_pathToErrorView);
            }

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return Redirect("/charityMakers/Index");
        }


        public async Task<IActionResult> Table()
        {
            var url = _urlBilder.CreatePost(_apiPath);
            IEnumerable<CharityMakerDTO> charityMakerContainer = null;

            var adderssUrl = _urlAdressBuilder.CreatePost(_pathToAdressApi);
            IEnumerable<AddressDTO> addressDTOContainer = null;

            try
            {
                charityMakerContainer = await _serverDownloader.GetAllAsync(url, HttpContext.Session);
                addressDTOContainer = await _serverAddressDownloader.GetAllAsync(adderssUrl, HttpContext.Session);
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

            var charityMakers = charityMakerContainer.Select(charityMaker => new CharityMaker()
            {
                Birthday = charityMaker.Birthday,
                FullName = new FullName()
                {
                    Name = charityMaker.Name,
                    Patronymic = charityMaker.Patronymic,
                    Surname = charityMaker.Surname
                },
                Address = new Address()
                {
                    Country = addressDTOContainer.FirstOrDefault(p => p.ID == charityMaker.AdressID).Country,
                    Region = addressDTOContainer.FirstOrDefault(p => p.ID == charityMaker.AdressID).Region,
                    City = addressDTOContainer.FirstOrDefault(p => p.ID == charityMaker.AdressID).City,
                    Street = addressDTOContainer.FirstOrDefault(p => p.ID == charityMaker.AdressID).Street,
                    House = addressDTOContainer.FirstOrDefault(p => p.ID == charityMaker.AdressID).House
                },
                ID = charityMaker.ID,
                Avatar = charityMaker.PhotoPath,
                EmailID = charityMaker.EmailID,
                Rating = charityMaker.Rating,
            });

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);

            return View(charityMakers);
        }
    }
}
