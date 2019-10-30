using System.Threading.Tasks;
using FamilyNetServer.Models.Identity;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FamilyNetServer.Controllers
{
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

        protected Task<ApplicationUser> GetCurrentUserAsync() => _unitOfWorkAsync.UserManager.GetUserAsync(HttpContext.User);

    }
}