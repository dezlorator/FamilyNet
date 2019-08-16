using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNet.Models.Identity;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamilyNet.Controllers
{
    public class BaseController : Controller
    {
        protected IUnitOfWorkAsync _unitOfWorkAsync;

        public BaseController(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWorkAsync = unitOfWork;
        }

        protected Task<ApplicationUser> GetCurrentUserAsync() => _unitOfWorkAsync.UserManager.GetUserAsync(HttpContext.User);

        protected async Task Check() // TODO : rewrite name
        {
            var user = await GetCurrentUserAsync();

            if (!(HttpContext.User.IsInRole("Admin") || user.HasPerson))
            {
                RedirectToAction("Index", "Home");
            }
        }

        protected async Task<IActionResult> CheckById(int id) // TODO : rewrite name
        {
            var user = await GetCurrentUserAsync();

            if (!(HttpContext.User.IsInRole("Admin") || user.HasPerson) || user.PersonID != id)
            {
                return RedirectToAction("Index", "Home");
            }

            return null;
        }

        
    }
}