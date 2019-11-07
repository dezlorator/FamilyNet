using FamilyNetLogs.Models;
using System.Collections.Generic;

namespace FamilyNetLogs.ViewModels
{
    public class LogsViewModel
    {
        public IEnumerable<Log> Logs { get; set; }
        public LogsPageInfo LogsPageInfo { get; set; }
    }
}