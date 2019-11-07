using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FamilyNetLogs.ViewModels;
using FamilyNetLogs.Database;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ReflectionIT.Mvc.Paging;
using FamilyNetLogs.Models;
using System;

namespace FamilyNetLogs.Controllers
{
    public class HomeController : Controller
    {
        private readonly FamilyNetLogsContext _context;

        public HomeController(FamilyNetLogsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string userId, string level,
                                               int page = 1, int rows = 6)
        {
            ViewData["userFilter"] = userId;
            ViewData["levelFilter"] = level;

            var logs = _context.Log.AsNoTracking();

            if (!String.IsNullOrEmpty(userId))
            {
                logs = logs.Where(l => l.UserId == userId);
            }

            if (!String.IsNullOrEmpty(level))
            {
                logs = logs.Where(l => l.Level.Contains(level));
            }

            var model = await PagingList.CreateAsync(logs.OrderBy(l => l.Id),
                rows, page);

            return View(model);
        }

        public IActionResult Error()
        {
            var message = new ErrorViewModel()
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(message);
        }
    }
}
