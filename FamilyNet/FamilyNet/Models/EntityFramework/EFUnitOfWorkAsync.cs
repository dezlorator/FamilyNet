using FamilyNet.Infrastructure;
using FamilyNet.Models.Identity;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace FamilyNet.Models.EntityFramework
{
    public class EFUnitOfWorkAsync : IUnitOfWorkAsync
    {
        #region Private fields

        private readonly ApplicationDbContext _context;

        #endregion

        #region Constructors

        public EFUnitOfWorkAsync(ApplicationDbContext cont,
                                 IUserValidator<ApplicationUser> userValid,
                                 IPasswordValidator<ApplicationUser> passValid,
                                 IPasswordHasher<ApplicationUser> passwordHash,
                                 ApplicationUserManager userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _context = cont;
            Donations = new EFRepositoryAsync<Donation>(cont);
            Orphanages = new OrphanageRepositoryAsync(cont);
            PasswordHasher = passwordHash;
            UserValidator = userValid;
            PasswordValidator = passValid;
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            TypeBaseItems = cont.TypeBaseItems; // TODO : rewrite this
        }

        #endregion

        #region Property

        public IOrphanageAsyncRepository Orphanages { get; set; }
        
        public IAsyncRepository<Donation> Donations { get; set; }

        public DbSet<TypeBaseItem> TypeBaseItems { get; set; } // TODO : rewrite this

        public IUserValidator<ApplicationUser> UserValidator { get; set; }

        public IPasswordValidator<ApplicationUser> PasswordValidator { get; set; }

        public IPasswordHasher<ApplicationUser> PasswordHasher { get; set; }

        public UserManager<ApplicationUser> UserManager { get; set; }

        public SignInManager<ApplicationUser> SignInManager { get; set; }

        public RoleManager<IdentityRole> RoleManager { get; set; }

        public IAsyncRepository<BaseItemType> BaseItemTypes { get; set; }

        public FamilyNetPhoneValidator PhoneValidator { get; set; }
        #endregion

        public void SaveChangesAsync()
        {
            _context.SaveChanges();
        }
    }
}
