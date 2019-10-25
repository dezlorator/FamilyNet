using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Identity;
using System.Text.RegularExpressions;


namespace FamilyNetServer.Infrastructure
{
    public class FamilyNetServerPhoneValidator : UserValidator<ApplicationUser>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser applicationUser)
        {
            IdentityResult result = await base.ValidateAsync(manager, applicationUser);
            List<IdentityError> errors = result.Succeeded ?
                new List<IdentityError>() : result.Errors.ToList();

            Regex regex = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");

            if (!regex.IsMatch(applicationUser.PhoneNumber))
            {
                errors.Add(new IdentityError
                {
                    Code = "PhoneError",
                    Description = "Некорректно введен номер телефона"
                });
            }
            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());

        }
    }
}
