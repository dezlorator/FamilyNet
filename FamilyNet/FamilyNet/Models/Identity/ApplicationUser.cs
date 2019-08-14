using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        [NotMapped]
        public Person GetEntity { get; set; }

        [NotMapped]
        public bool HasPerson
        {
            get
            {
                return PersonID != null && PersonType != PersonType.User;
            }
        }

        public int? PersonID { get; set; }

        public PersonType PersonType { get; set; }

    }

    public enum PersonType
    {
        User,
        CharityMaker,
        Volunteer,
        Orphan,
        Representative
        
    }
}

    

