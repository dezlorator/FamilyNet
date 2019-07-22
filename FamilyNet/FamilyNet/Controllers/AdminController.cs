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

namespace FamilyNet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
                
        public AdminController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork)
        {

        }
        public ViewResult Index() => View(_unitOfWorkAsync.UserManager.Users);
        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.Phone
                    
                };
                IdentityResult result
                    = await _unitOfWorkAsync.UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await _unitOfWorkAsync.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _unitOfWorkAsync.UserManager.DeleteAsync(user);
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
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", _unitOfWorkAsync.UserManager.Users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            ApplicationUser user = await _unitOfWorkAsync.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser us, string password)
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
                if ((validEmail.Succeeded && validPass == null)
                        || (validEmail.Succeeded
                        && password != string.Empty && validPass.Succeeded)|| (validEmail.Succeeded && validPhone.Succeeded && password!= string.Empty && validPhone.Succeeded)
                        ||(validEmail.Succeeded && validPass == null && validPhone.Succeeded))
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
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(user);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
