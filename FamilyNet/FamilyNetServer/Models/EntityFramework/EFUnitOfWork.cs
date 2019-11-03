using FamilyNetServer.Infrastructure;
using FamilyNetServer.Models.Identity;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace FamilyNetServer.Models.EntityFramework
{
    public class EFUnitOfWork : IUnitOfWork
    {
        #region Private fields

        private readonly ApplicationDbContext _context;
       
        #endregion

        #region Constructors

        public EFUnitOfWork(ApplicationDbContext cont,
                                 IUserValidator<ApplicationUser> userValid, 
                                 IPasswordValidator<ApplicationUser> passValid, 
                                 IPasswordHasher<ApplicationUser> passwordHash, 
                                 ApplicationUserManager userManager,
                                 SignInManager<ApplicationUser> signInManager, 
                                 RoleManager<IdentityRole> roleManager)
        {
            _context = cont;
            Address = new EFRepository<Address>(cont);
            Location = new EFRepository<Location>(cont);
            CharityMakers = new EFRepository<CharityMaker>(cont);
            Donations = new EFRepository<Donation>(cont);
            Orphanages = new OrphanageRepositoryAsync(cont);
            Orphans = new EFRepository<Orphan>(cont);
            DonationItems = new EFRepository<DonationItem>(cont);
            Representatives = new EFRepository<Representative>(cont);
            Volunteers = new EFRepository<Volunteer>(cont);
            Availabilities = new EFRepository<Availability>(cont);
            PasswordHasher = passwordHash;
            UserValidator = userValid;
            PasswordValidator = passValid;
            PhoneValidator = new FamilyNetServerPhoneValidator();
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            BaseItemTypes = new EFRepository<BaseItemType>(cont);
            TypeBaseItems = cont.TypeBaseItems; // TODO : rewrite this
        }

        #endregion

        #region Property

        public IOrphanageRepository Orphanages { get; set; }

        public IRepository<Address> Address { get; set; }
        public IRepository<Location> Location { get; set; }

        public IRepository<CharityMaker> CharityMakers { get; set; }

        public IRepository<Representative> Representatives { get; set; }

        public IRepository<Volunteer> Volunteers { get; set; }

        public IRepository<Donation> Donations { get; set; }

        public DbSet<TypeBaseItem> TypeBaseItems { get; set; } // TODO : rewrite this

        public IRepository<Orphan> Orphans { get; set; }
        public IRepository<Availability> Availabilities { get; set; }
        public IUserValidator<ApplicationUser> UserValidator { get; set; }

        public IPasswordValidator<ApplicationUser> PasswordValidator { get; set; }

        public IPasswordHasher<ApplicationUser> PasswordHasher { get; set; }

        public FamilyNetServerPhoneValidator PhoneValidator { get; set; }

        public UserManager<ApplicationUser> UserManager { get; set; }

        public SignInManager<ApplicationUser> SignInManager { get; set; }

        public RoleManager<IdentityRole> RoleManager { get; set; }
        public IRepository<DonationItem> DonationItems { get; set; }

        public IRepository<BaseItemType> BaseItemTypes { get; set; }

        #endregion

        public void SaveChangesAsync()
        {
            _context.SaveChanges();
        }
    }
}
