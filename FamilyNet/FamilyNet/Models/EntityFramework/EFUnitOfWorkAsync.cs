using FamilyNet.Infrastructure;
using FamilyNet.Models.Identity;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.EntityFramework
{
    public class EFUnitOfWorkAsync : IUnitOfWorkAsync
    {
        #region Private fields

        private readonly ApplicationDbContext _context;
       
        #endregion

        #region Constructors

        public EFUnitOfWorkAsync(ApplicationDbContext cont, IUserValidator<ApplicationUser> userValid, IPasswordValidator<ApplicationUser> passValid, 
            IPasswordHasher<ApplicationUser> passwordHash, ApplicationUserManager userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = cont;
            CharityMakers = new EFRepositoryAsync<CharityMaker>(cont);
            Donations = new EFRepositoryAsync<Donation>(cont);
            Orphanages = new OrphanageRepositoryAsync(cont);
            Orphans = new EFRepositoryAsync<Orphan>(cont);
            Representatives = new EFRepositoryAsync<Representative>(cont);
            Volunteers = new EFRepositoryAsync<Volunteer>(cont);
            PasswordHasher = passwordHash;
            UserValidator = userValid;
            PasswordValidator = passValid;
            PhoneValidator = new FamilyNetPhoneValidator();
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            BaseItemTypes = new EFRepositoryAsync<BaseItemType>(cont);
            TypeBaseItems = new TypeBaseItemRepository(cont); // TODO : rewrite this
        }



        #endregion

        #region Property

        public IOrphanageAsyncRepository Orphanages { get; set; }

        public IAsyncRepository<CharityMaker> CharityMakers { get; set; }

        public IAsyncRepository<Representative> Representatives { get; set; }

        public IAsyncRepository<Volunteer> Volunteers { get; set; }

        public IAsyncRepository<Donation> Donations { get; set; }

        public TypeBaseItemRepository TypeBaseItems { get; set; } // TODO : rewrite this

        public IAsyncRepository<Orphan> Orphans { get; set; }

        public IUserValidator<ApplicationUser> UserValidator { get; set; }

        public IPasswordValidator<ApplicationUser> PasswordValidator { get; set; }

        public IPasswordHasher<ApplicationUser> PasswordHasher { get; set; }

        public FamilyNetPhoneValidator PhoneValidator { get; set; }

        public UserManager<ApplicationUser> UserManager { get; set; }

        public SignInManager<ApplicationUser> SignInManager { get; set; }

        public RoleManager<IdentityRole> RoleManager { get; set; }

        public IAsyncRepository<BaseItemType> BaseItemTypes { get; set; }

        public void SaveChangesAsync()
        {
            _context.SaveChanges();
        }

        #endregion
    }
}
