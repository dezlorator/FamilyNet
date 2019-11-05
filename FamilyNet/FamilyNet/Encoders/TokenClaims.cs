using System;
using System.Collections.Generic;

namespace FamilyNet.Encoders
{
    public class TokenClaims
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; }
        public string Email { get; set; }
        public int PersonId { get; set; }
    }
}
