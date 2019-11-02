using FamilyNet.Infrastructure;
using FamilyNet.Models.Identity;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
namespace FamilyNet.Models.EntityFramework
{
    public class EFUnitOfWork : IIdentity
    {
        #region Constructors

        public EFUnitOfWork(IUserValidator<ApplicationUser> userValid,
                            IPasswordValidator<ApplicationUser> passValid,
                            IPasswordHasher<ApplicationUser> passwordHash,
                            ApplicationUserManager userManager,
                            SignInManager<ApplicationUser> signInManager,
                            RoleManager<IdentityRole> roleManager)
        {
            PasswordHasher = passwordHash;
            UserValidator = userValid;
            PasswordValidator = passValid;
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        #endregion

        #region Property

        public IUserValidator<ApplicationUser> UserValidator { get; set; }

        public IPasswordValidator<ApplicationUser> PasswordValidator { get; set; }

        public IPasswordHasher<ApplicationUser> PasswordHasher { get; set; }

        public UserManager<ApplicationUser> UserManager { get; set; }

        public SignInManager<ApplicationUser> SignInManager { get; set; }

        public RoleManager<IdentityRole> RoleManager { get; set; }

        public FamilyNetPhoneValidator PhoneValidator { get; set; }

        #endregion
    }
}
