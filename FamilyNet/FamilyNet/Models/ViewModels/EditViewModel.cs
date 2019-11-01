using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
namespace FamilyNet.Models.ViewModels
{
    public class EditViewModel
    {
        public string Id { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        public List<IdentityRole> AllRoles { get; set; }

        public IList<string> UserRoles { get; set; }

        public EditViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
