using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FamilyNetLogs.ViewModels;
using FamilyNetLogs.Database;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FamilyNetLogs.Controllers
{
    public class HomeController : Controller
    {
        private readonly FamilyNetLogsContext _context;

        public HomeController(FamilyNetLogsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _context.Log.ToListAsync();
            return View(logs);
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
