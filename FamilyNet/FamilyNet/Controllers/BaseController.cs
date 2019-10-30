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

        protected IUnitOfWorkAsync _unitOfWork;
        protected IStringLocalizer<SharedResource> _sharedLocalizer;

        #endregion

        #region ctor

        public BaseController(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public BaseController(IUnitOfWorkAsync unitOfWork, IStringLocalizer<SharedResource> sharedLocalizer)
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