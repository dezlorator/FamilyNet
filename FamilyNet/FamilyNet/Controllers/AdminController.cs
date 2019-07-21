using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using FamilyNet.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Infrastructure;

namespace FamilyNet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private IUserValidator<ApplicationUser> _userValidator;
        private IPasswordValidator<ApplicationUser> _passwordValidator;
        private IPasswordHasher<ApplicationUser> _passwordHasher;
        private FamilyNetPhoneValidator _phoneValidator;
        
        public AdminController(UserManager<ApplicationUser> usrMgr, IUserValidator<ApplicationUser> userValid, IPasswordValidator<ApplicationUser> passValid, IPasswordHasher<ApplicationUser> passwordHash)
        {
            _userManager = usrMgr;
            _userValidator = userValid;
            _passwordValidator = passValid;
            _passwordHasher = passwordHash;
        }
        public ViewResult Index() => View(_userManager.Users);
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
                    = await _userManager.CreateAsync(user, model.Password);

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
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
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
            return View("Index", _userManager.Users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
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
        public async Task<IActionResult> Edit(string id, string email,string phone,
                string password)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;
                IdentityResult validEmail
                    = await _userValidator.ValidateAsync(_userManager, user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }
                user.PhoneNumber = phone;
                IdentityResult validPhone
                    = await _phoneValidator.ValidateAsync(_userManager, user);
                if (!validPhone.Succeeded)
                {
                    AddErrorsFromResult(validPhone);
                }


                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await _passwordValidator.ValidateAsync(_userManager,
                        user, password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user,
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
                    IdentityResult result = await _userManager.UpdateAsync(user);
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
