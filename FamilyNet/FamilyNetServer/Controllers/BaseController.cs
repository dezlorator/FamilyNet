using System.Threading.Tasks;
using FamilyNetServer.Models.Identity;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FamilyNetServer.Controllers
{
    public class BaseController : Controller
    {
        protected IUnitOfWork _unitOfWork;
        protected IStringLocalizer<SharedResource> _sharedLocalizer;


        public BaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public BaseController(IUnitOfWork unitOfWork, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _unitOfWork = unitOfWork;
            _sharedLocalizer = sharedLocalizer;
        }

        protected Task<ApplicationUser> GetCurrentUserAsync() => _unitOfWork.UserManager.GetUserAsync(HttpContext.User);

    }
}