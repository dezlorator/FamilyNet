using System.Threading.Tasks;
using FamilyNet.Models.Identity;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FamilyNet.Controllers
{
    public class BaseController : Controller
    {
        #region fields

        protected IIdentity _unitOfWork;
        protected IStringLocalizer<SharedResource> _sharedLocalizer;

        #endregion

        #region ctor

        public BaseController(IIdentity unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public BaseController(IIdentity unitOfWork, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _unitOfWork = unitOfWork;
            _sharedLocalizer = sharedLocalizer;
        }

        protected Task<ApplicationUser> GetCurrentUserAsync() => _unitOfWork.UserManager.GetUserAsync(HttpContext.User);

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

            if (!(HttpContext.User.IsInRole("Admin") || (user.HasPerson && user.PersonID == id)) )
            {
                return RedirectToAction("Index", "Home");
            }

            return null;
        }


    }
}