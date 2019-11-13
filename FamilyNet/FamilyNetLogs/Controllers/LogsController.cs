using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FamilyNetLogs.ViewModels;
using FamilyNetLogs.Database;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ReflectionIT.Mvc.Paging;
using System;

namespace FamilyNetLogs.Controllers
{
    public class LogsController : Controller
    {
        private readonly FamilyNetLogsContext _context;

        public LogsController(FamilyNetLogsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string userId, string level,
                                               string token, int page = 1,
                                               int rows = 7)
        {
            ViewData["userFilter"] = userId;
            ViewData["levelFilter"] = level;
            ViewData["tokenFilter"] = token;

            var logs = _context.Log.AsNoTracking();

            if (!String.IsNullOrEmpty(token))
            {
                logs = logs.Where(l => l.Token.Contains(token));
            }

            if (!String.IsNullOrEmpty(userId))
            {
                logs = logs.Where(l => l.UserId == userId);
            }


            if (!String.IsNullOrEmpty(level))
            {
                logs = logs.Where(l => l.Level.Contains(level));
            }

            var model = await PagingList.CreateAsync(logs.OrderByDescending(l => l.Id),
                rows, page);

            return View(model);
        }

        public async Task<IActionResult> JSON(int id)
        {
            var log = await _context.Log.FirstOrDefaultAsync(l => l.Id == id);

            return View(log);
        }

        public async Task<IActionResult> Exception(int id)
        {
            var log = await _context.Log.FirstOrDefaultAsync(l => l.Id == id);

            return View(log);
        }

        [HttpGet]
        public IActionResult DeleteLogs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            _context.Log.RemoveRange(_context.Log);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
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
