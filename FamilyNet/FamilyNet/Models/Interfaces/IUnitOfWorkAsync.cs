using FamilyNet.Infrastructure;
using FamilyNet.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace FamilyNet.Models.Interfaces
{
    public interface IUnitOfWorkAsync
    {
        IOrphanageAsyncRepository Orphanages { get; }
        IAsyncRepository<CharityMaker> CharityMakers { get; }
        IAsyncRepository<Representative> Representatives { get; }
        IAsyncRepository<Volunteer> Volunteers { get; }
        IAsyncRepository<Donation> Donations { get; }
        IAsyncRepository<Orphan> Orphans { get; }

        IUserValidator<ApplicationUser> UserValidator { get; }
        IPasswordValidator<ApplicationUser> PasswordValidator { get; }
        IPasswordHasher<ApplicationUser> PasswordHasher { get; }
        FamilyNetPhoneValidator PhoneValidator { get; }
        UserManager<ApplicationUser> UserManager { get; }
        SignInManager<ApplicationUser> SignInManager { get; }
        RoleManager<IdentityRole> RoleManager { get; }
        void SaveChangesAsync();
    }
}