using System;

namespace FamilyNetLogs.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime Logged { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string CallSite { get; set; }
        public string UserId { get; set; }
        public string JSON { get; set; }
        public string Exception { get; set; }
        public string Status { get; set; }
        public string Token { get; set; }
        public string Info { get; set; }
    }
}
