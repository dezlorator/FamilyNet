using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FamilyNetServer.Models;
using FamilyNetServer.Models.ViewModels;
using FamilyNetServer.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using FamilyNetServer.Models.Interfaces;

namespace FamilyNetServer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
                
        public AdminController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        public ViewResult Index() => View(_unitOfWork.UserManager.Users);
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
                        = await _unitOfWork.UserManager.CreateAsync(user, model.Password);

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
            ApplicationUser user = await _unitOfWork.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _unitOfWork.UserManager.DeleteAsync(user);
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
            return View("Index", _unitOfWork.UserManager.Users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            ApplicationUser user = await _unitOfWork.UserManager.FindByIdAsync(id);
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
            ApplicationUser user = await _unitOfWork.UserManager.FindByIdAsync(us.Id);
            if (user != null)
            {
                user.Email = us.Email;

                IdentityResult validEmail
                    = await _unitOfWork.UserValidator.ValidateAsync(_unitOfWork.UserManager, user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }
                user.PhoneNumber = us.PhoneNumber;
                IdentityResult validPhone
                    = await _unitOfWork.PhoneValidator.ValidateAsync(_unitOfWork.UserManager, user);
                if (!validPhone.Succeeded)
                {
                    AddErrorsFromResult(validPhone);
                }



                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await _unitOfWork.PasswordValidator.ValidateAsync(_unitOfWork.UserManager,
                        user, password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = _unitOfWork.PasswordHasher.HashPassword(user,
                            password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }



                if ((validEmail.Succeeded  && validPhone.Succeeded))

                {
                    if ((validPass != null && validEmail.Succeeded && password != string.Empty && validPass.Succeeded && validPhone.Succeeded))
                    {
                        IdentityResult result = await _unitOfWork.UserManager.UpdateAsync(user);
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

                        IdentityResult result = await _unitOfWork.UserManager.UpdateAsync(user);
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
            SeedData seedData = new SeedData(_unitOfWork);
            seedData.EnsurePopulated();

            return Redirect("/Home/Index");
        }
    }
}
