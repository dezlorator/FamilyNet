using Microsoft.AspNetCore.Mvc;
using System;

namespace FamilyNetLogs.ViewModels
{
    public class LogsPageInfo
    {
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; }
        public int PageSize { get; set; } = 6;
        public int Count { get; set; }
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
    }
}
