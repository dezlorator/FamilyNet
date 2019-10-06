using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNet.Models.Identity;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FamilyNet.Controllers
{
    /// <summary>
    /// The base controller in project
    /// </summary>
    public class BaseController : Controller
    {
        protected IUnitOfWorkAsync _unitOfWorkAsync;
        protected IStringLocalizer<SharedResource> _sharedLocalizer;


        public BaseController(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWorkAsync = unitOfWork;
        }

        public BaseController(IUnitOfWorkAsync unitOfWork, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _unitOfWorkAsync = unitOfWork;
            _sharedLocalizer = sharedLocalizer;
        }

        /// <summary>
        /// Method provides current authorized user
        /// </summary>
        /// <returns>authorized user</returns>
        protected Task<ApplicationUser> GetCurrentUserAsync() => _unitOfWorkAsync.UserManager.GetUserAsync(HttpContext.User);

        /// <summary>
        /// Method checks current role
        /// </summary>
        /// <returns>Redirect to Home/Index page if authorized user is not 
        /// Admin,CharityMaker, Volunteer, Orphan, Representative</returns>
        protected async Task Check() // TODO : rewrite name
        {
            var user = await GetCurrentUserAsync();

            if (!(HttpContext.User.IsInRole("Admin") || user.HasPerson))
            {
                RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Method checks valid authorized user role, id
        /// </summary>
        /// <param name="id">user's identifier</param>
        /// <returns>Redirect to /Home/Index if authorized user role, id are not valid
        /// or return null</returns>
        protected async Task<IActionResult> CheckById(int id) // TODO : rewrite name
        {
            var user = await GetCurrentUserAsync();

            if (!(HttpContext.User.IsInRole("Admin") || (user.HasPerson && user.PersonID == id)) )
            {
                return RedirectToAction("Index", "Home");
            }

            return null;
        }


    }
}