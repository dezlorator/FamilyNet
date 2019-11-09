using System;
using System.Collections.Generic;

namespace DataTransferObjects
{
    public class TokenDTO
    {
        public string Token { get; set; }
        public IList<string> Roles { get; set; }
        public string Email { get; set; }
        public int? PersonId { get; set; }
        public string Id { get; set; }
    }
}
