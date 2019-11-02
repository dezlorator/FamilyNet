using FamilyNetServer.Infrastructure;
using FamilyNetServer.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace FamilyNetServer.Models.Interfaces
{
    public interface IIdentity
    {

        IUserValidator<ApplicationUser> UserValidator { get; }

        IPasswordValidator<ApplicationUser> PasswordValidator { get; }

        IPasswordHasher<ApplicationUser> PasswordHasher { get; }

        FamilyNetServerPhoneValidator PhoneValidator { get; }

        UserManager<ApplicationUser> UserManager { get; }

        SignInManager<ApplicationUser> SignInManager { get; }

        RoleManager<IdentityRole> RoleManager { get; }



    }
}