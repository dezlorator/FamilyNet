using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using FamilyNet.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Infrastructure;
using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
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
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        ServerSimpleDataDownloader<UserDTO> _downloader;
        private readonly string _apiPath = "http://localhost:53605/api/v1/users/";
        public AdminController(IUnitOfWorkAsync unitOfWork, ServerSimpleDataDownloader<UserDTO> downloader)
                              : base(unitOfWork)
        {
            _downloader = downloader;
        }

        public async Task<IActionResult> Index()
        {
            var url = _apiPath;
            IEnumerable<UserDTO> userDTO = null;

            try
            {
                userDTO = await _downloader.GetAllAsync(url);
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



            var users = userDTO.Select(user => new ApplicationUser()
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            });


            return View(users);
        }
        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            var url = _apiPath;
            var status = await _downloader.CreatePostAsync(url, model);

            if (status.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            return Redirect("/Admin/Index");


        }

        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _apiPath + id;

            try
            {
                var status = await _downloader.DeleteAsync(url);

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

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(string id)
        {
            ApplicationUser user = await _unitOfWorkAsync.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                EditViewModel editView = new EditViewModel { Id = user.Id, Email = user.Email, PhoneNumber = user.PhoneNumber };
                return View(editView);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel us, string password)
        {
            ApplicationUser user = await _unitOfWorkAsync.UserManager.FindByIdAsync(us.Id);
            if (user != null)
            {
                user.Email = us.Email;

                IdentityResult validEmail
                    = await _unitOfWorkAsync.UserValidator.ValidateAsync(_unitOfWorkAsync.UserManager, user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }
                user.PhoneNumber = us.PhoneNumber;
                IdentityResult validPhone
                    = await _unitOfWorkAsync.PhoneValidator.ValidateAsync(_unitOfWorkAsync.UserManager, user);
                if (!validPhone.Succeeded)
                {
                    AddErrorsFromResult(validPhone);
                }



                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await _unitOfWorkAsync.PasswordValidator.ValidateAsync(_unitOfWorkAsync.UserManager,
                        user, password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = _unitOfWorkAsync.PasswordHasher.HashPassword(user,
                            password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }



                if ((validEmail.Succeeded && validPhone.Succeeded))

                {
                    if ((validPass != null && validEmail.Succeeded && password != string.Empty && validPass.Succeeded && validPhone.Succeeded))
                    {
                        IdentityResult result = await _unitOfWorkAsync.UserManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                    else
                    {

                        IdentityResult result = await _unitOfWorkAsync.UserManager.UpdateAsync(user);
                        if (result.Succeeded && validPass.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(us);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        public IActionResult SeedData()
        {
            SeedData seedData = new SeedData(_unitOfWorkAsync);
            seedData.EnsurePopulated();

            return Redirect("/Home/Index");
        }
    }
}
