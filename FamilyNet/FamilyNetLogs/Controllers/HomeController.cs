using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FamilyNetLogs.ViewModels;
using FamilyNetLogs.Database;
using System.Linq;


namespace FamilyNetLogs.Controllers
{
    public class HomeController : Controller
    {
        private readonly FamilyNetLogsContext _context;

        public HomeController(FamilyNetLogsContext context)
        {
            _context = context;
        }

        [Route("/{page}")]
        public IActionResult Index(int page = 1, int rows = 6)
        {
            var logsPageInfo = new LogsPageInfo
            {
                CurrentPage = page,
                PageSize = 6,
                Count = _context.Log.Count()
            };

            var logs = _context.Log.Skip((logsPageInfo.CurrentPage - 1) * logsPageInfo.PageSize)
                .Take(logsPageInfo.PageSize);

            var model = new LogsViewModel()
            {
                Logs = logs,
                LogsPageInfo = logsPageInfo
            };

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
