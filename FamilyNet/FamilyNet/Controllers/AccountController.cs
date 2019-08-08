using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using FamilyNet.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyNet.Controllers
{
    
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork)
        {
          
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var allRoles = _unitOfWorkAsync.RoleManager.Roles.ToList();
            var yourDropdownList = new SelectList(allRoles.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Name
            }).ToList(), "Value", "Text");
            var viewModel = new RegisterViewModel()
            {
                // The Dropdownlist values
                YourDropdownList = yourDropdownList
            };
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var allRoles = _unitOfWorkAsync.RoleManager.Roles.ToList();
            var yourDropdownList = new SelectList(allRoles.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Name
            }).ToList(), "Value", "Text");
            model.YourDropdownList = yourDropdownList;
            if (ModelState.IsValid)
            {

                
                ApplicationUser user = new ApplicationUser { Email = model.Email, UserName = model.Email, PhoneNumber = model.Phone };
                // добавляем пользователя.
                var result = await _unitOfWorkAsync.UserManager.CreateAsync(user, model.Password);

                await _unitOfWorkAsync.UserManager.AddToRoleAsync(user, model.YourDropdownSelectedValue);

                


                if (result.Succeeded)
                {
                    
                    // установка куки.
                    
                    
                    await _unitOfWorkAsync.SignInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            
            return View(model);
        }

        //В Get-версии метода Login мы получаем адрес для возврата в виде параметра returnUrl и передаем его в модель LoginViewModel.
        //    В Post-версии метода Login получаем данные из представления в виде модели LoginViewModel.
        //    Всю работу по аутентификации пользователя выполняет метод signInManager.PasswordSignInAsync().
        //    Этот метод принимает логин и пароль пользователя.Третий параметр метода указывает, надо ли сохранять устанавливаемые куки на долгое время.

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _unitOfWorkAsync.UserManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _unitOfWorkAsync.SignInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result =
                            await _unitOfWorkAsync.SignInManager.PasswordSignInAsync(
                                user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Такого пользователя не существует, зарегистрируйтесь, пожалуйста!");
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _unitOfWorkAsync.SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}
