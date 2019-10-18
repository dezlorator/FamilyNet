﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Identity;
using System.Text.RegularExpressions;

namespace FamilyNetServer.Infrastructure
{
    public class FamilyNetServerPasswordValidator : PasswordValidator<ApplicationUser>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser>manager, ApplicationUser applicationUser, string password)
        {
            IdentityResult result = await base.ValidateAsync(manager, applicationUser, password);

            List<IdentityError> errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();
            //if (password.ToLower().Contains(applicationUser.UserName.ToLower()))
            //{
            //    errors.Add(new IdentityError
            //    {
            //        Code = "PasswordContainsUserName",
            //        Description = "Password cannot contain username"
            //    });
            //}
            if (password.Contains("12345"))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordContainsSequence",
                    Description = "Password cannot contain numeric sequence"
                });
            }
            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
        
    }
}
