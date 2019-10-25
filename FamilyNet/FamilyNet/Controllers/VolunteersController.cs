using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using FamilyNet.Models.ViewModels;
using DataTransferObjects;
using FamilyNet.Downloader;
using Microsoft.Extensions.Localization;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using FamilyNet.StreamCreater;

namespace FamilyNet.Controllers
{
    public class VolunteersController : BaseController
    {
        #region private fields

        private readonly IStringLocalizer<VolunteersController> _localizer;
        private readonly ServerDataDownloader<VolunteerDTO> _downloader;
        private readonly IServerAddressDownloader _addressDownloader;
        private readonly IURLVolunteersBuilder _URLVolunteersBuilder;
        private readonly IURLAddressBuilder _URLAddressBuilder;
        private readonly string _apiPath = "api/v1/volunteers";
        private readonly string _apiAddressPath = "api/v1/address";
        private readonly IFileStreamCreater _streamCreater;

        #endregion

        #region ctor

        public VolunteersController(IUnitOfWorkAsync unitOfWork,
                                 IStringLocalizer<VolunteersController> localizer,
                                 ServerDataDownloader<VolunteerDTO> downLoader,
                                 IServerAddressDownloader addressDownloader,
                                 IURLVolunteersBuilder URLVolunteersBuilder,
                                 IURLAddressBuilder URLAddressBuilder,
                                 IFileStreamCreater streamCreater)
            : base(unitOfWork)
        {
            _localizer = localizer;
            _downloader = downLoader;
            _addressDownloader = addressDownloader;
            _URLVolunteersBuilder = URLVolunteersBuilder;
            _URLAddressBuilder = URLAddressBuilder;
            _streamCreater = streamCreater;
        }

        #endregion

        public async Task<IActionResult> Index(int id, PersonSearchModel searchModel)
        {
            var url = _URLVolunteersBuilder.GetAllWithFilter(_apiPath,
                                                           searchModel,
                                                           id);
            IEnumerable<VolunteerDTO> volunteers = null;

            try
            {
                volunteers = await _downloader.GetAllAsync(url, HttpContext.Session);
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

            var selectedVolunteers = volunteers.Select(volunteer => new Volunteer()
            {
                Birthday = volunteer.Birthday,
                FullName = new FullName()
                {
                    Name = volunteer.Name,
                    Patronymic = volunteer.Patronymic,
                    Surname = volunteer.Surname
                },
                ID = volunteer.ID,
                Avatar = volunteer.PhotoPath,
                AddressID = volunteer.AddressID,
                EmailID = volunteer.EmailID,
                Rating = volunteer.Rating
            });

            return View(selectedVolunteers);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLVolunteersBuilder.GetById(_apiPath, id.Value);
            VolunteerDTO volunteerDTO = null;

            try
            {
                volunteerDTO = await _downloader.GetByIdAsync(url, HttpContext.Session);
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

            if (volunteerDTO == null)
            {
                return NotFound();
            }

            var addressUrl = _URLAddressBuilder.GetById(_apiAddressPath, (int)volunteerDTO.AddressID);
            AddressDTO adderessDTO = null;

            try
            {
                adderessDTO = await _addressDownloader.GetByIdAsync(addressUrl, HttpContext.Session);
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

            if (volunteerDTO == null)
            {
                return NotFound();
            }

            var volunteer = new Volunteer()
            {
                Birthday = volunteerDTO.Birthday,
                FullName = new FullName()
                {
                    Name = volunteerDTO.Name,
                    Patronymic = volunteerDTO.Patronymic,
                    Surname = volunteerDTO.Surname
                },
                ID = volunteerDTO.ID,
                Avatar = volunteerDTO.PhotoPath,
                AddressID = volunteerDTO.AddressID,
                Address = new Address()
                {
                    Country = adderessDTO.Country,
                    Region = adderessDTO.Region,
                    City = adderessDTO.City,
                    Street = adderessDTO.Street,
                    House = adderessDTO.House
                },
                EmailID = volunteerDTO.EmailID,
                Rating = volunteerDTO.Rating,
            };

            return View(volunteer);
        }

        public async Task<IActionResult> Create()
        {
            await Check();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VolunteerDTO volunteerDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(volunteerDTO);
            }

            Stream stream = null;

            if (volunteerDTO.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(volunteerDTO.Avatar);
            }

            var addressUrl = _URLAddressBuilder.CreatePost(_apiAddressPath);
            var statusAddress = await _addressDownloader.CreatePostAsync(addressUrl,
                                            volunteerDTO.Address, HttpContext.Session);

            volunteerDTO.AddressID = statusAddress.Content.ReadAsAsync<AddressDTO>().Result.ID;
            var url = _URLVolunteersBuilder.CreatePost(_apiPath);
            var status = await _downloader.CreatePostAsync(url, volunteerDTO,
                                                             stream, volunteerDTO.Avatar?.FileName,
                                                             HttpContext.Session);

            if (status != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            return Redirect("/Volunteers/Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }

            var url = _URLVolunteersBuilder.GetById(_apiPath, id.Value);
            VolunteerDTO volunteerDTO = null;

            try
            {
                volunteerDTO = await _downloader.GetByIdAsync(url, HttpContext.Session);
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

            if (volunteerDTO.AddressID == null)
            {
                return View(volunteerDTO);
            }

            var addressURL = _URLAddressBuilder.GetById(_apiAddressPath, 
                                                (int)volunteerDTO.AddressID);
            AddressDTO addressDTO = null;

            try
            {
                addressDTO = await _addressDownloader.GetByIdAsync(addressURL, HttpContext.Session);
            }
            catch (ArgumentNullException)
            {
                return View(volunteerDTO);
            }
            catch (HttpRequestException)
            {
                return View(volunteerDTO);
            }
            catch (JsonException)
            {
                return View(volunteerDTO);
            }

            volunteerDTO.Address = addressDTO;

            return View(volunteerDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VolunteerDTO volunteerDTO)
        {
            if (id != volunteerDTO.ID)
            {
                return NotFound();
            }

            if (volunteerDTO.AddressID == null)
            {
                volunteerDTO.AddressID = volunteerDTO.Address.ID;
            }

            if (!ModelState.IsValid)
            {
                return View(volunteerDTO);
            }

            Stream stream = null;

            if (volunteerDTO.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(volunteerDTO.Avatar);
            }

            var addressUrl = _URLAddressBuilder.GetById(_apiAddressPath, (int)volunteerDTO.AddressID);
            var statusAddress = await _addressDownloader.CreatePutAsync(addressUrl,
                                            volunteerDTO.Address, HttpContext.Session);

            var url = _URLVolunteersBuilder.GetById(_apiPath, id);
            var status = await _downloader.CreatePutAsync(url, volunteerDTO,
                                                            stream, volunteerDTO.Avatar?.FileName,
                                                            HttpContext.Session);

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            return Redirect("/Volunteers/Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLVolunteersBuilder.GetById(_apiPath, id.Value);
            VolunteerDTO volunteerDTO = null;

            try
            {
                volunteerDTO = await _downloader.GetByIdAsync(url, HttpContext.Session);
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

            if (volunteerDTO == null)
            {
                return NotFound();
            }

            return View(id.Value);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var url = _URLVolunteersBuilder.GetById(_apiPath, id);
            var status = await _downloader.DeleteAsync(url, HttpContext.Session);

            if (status != HttpStatusCode.OK)
            {
                return Redirect("/Home/Error");
            }

            return Redirect("/Volunteers/Index");
        }

        public async Task<IActionResult> VolunteersTable(int id, PersonSearchModel searchModel)
        {
            var url = _URLVolunteersBuilder.GetAllWithFilter(_apiPath,
                                                           searchModel,
                                                           id);
            IEnumerable<VolunteerDTO> volunteers = null;

            var addressUrl = _URLAddressBuilder.CreatePost("api/v1/address");
            IEnumerable<AddressDTO> addresses = null;

            try
            {
                volunteers = await _downloader.GetAllAsync(url, HttpContext.Session);
                addresses = await _addressDownloader.GetAllAsync(addressUrl, HttpContext.Session);
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

            var selectedVolunteers = volunteers.Select(volunteer => new Volunteer()
            {
                Birthday = volunteer.Birthday,
                FullName = new FullName()
                {
                    Name = volunteer.Name,
                    Patronymic = volunteer.Patronymic,
                    Surname = volunteer.Surname
                },
                ID = volunteer.ID,
                Avatar = volunteer.PhotoPath,
                AddressID = volunteer.AddressID,
                Address = new Address()
                {
                    Country = addresses.FirstOrDefault(p => p.ID == volunteer.AddressID).Country,
                    Region = addresses.FirstOrDefault(p => p.ID == volunteer.AddressID).Region,
                    City = addresses.FirstOrDefault(p => p.ID == volunteer.AddressID).City,
                    Street = addresses.FirstOrDefault(p => p.ID == volunteer.AddressID).Street,
                    House = addresses.FirstOrDefault(p => p.ID == volunteer.AddressID).House
                },
                EmailID = volunteer.EmailID,
                Rating = volunteer.Rating
            });

            return View(selectedVolunteers);
        }
    }
}
