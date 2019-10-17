using FamilyNet.Infrastructure;
using FamilyNet.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace FamilyNet.Models.Interfaces
{
    public interface IIdentityAsync
    {

        IUserValidator<ApplicationUser> UserValidator { get; }

        IPasswordValidator<ApplicationUser> PasswordValidator { get; }

        IPasswordHasher<ApplicationUser> PasswordHasher { get; }

        FamilyNetPhoneValidator PhoneValidator { get; }

        UserManager<ApplicationUser> UserManager { get; }

        SignInManager<ApplicationUser> SignInManager { get; }

        RoleManager<IdentityRole> RoleManager { get; }



    }
}