using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FamilyNet.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [BindNever]
        public bool IsDeleted { get; set; } = false;
    }
}
