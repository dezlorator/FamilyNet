using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using FamilyNet.Models.Identity;

namespace FamilyNet.Models.Identity
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options)
        {

        }
        public static async Task CreateAdminAccount(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string email = configuration["Data:AdminUser:Email"];
            string password = configuration["Data:AdminUser:Password"];
            string role = configuration["Data:AdminUser:Role"];

            if (await userManager.FindByEmailAsync(email) == null)
            {
                if (await userManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
                ApplicationUser user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    PersonType = PersonType.User

                };

                user.EmailConfirmed = true;
                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
        public static async Task InitializeRolesAsync(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await RoleName(roleManager, "User");
            await RoleName(roleManager, "Orphan");
            await RoleName(roleManager, "Representative");
            await RoleName(roleManager, "Volunteer");
            await RoleName(roleManager, "CharityMaker");
        }
        public static async Task RoleName(RoleManager<IdentityRole> roleManager, string name)
        {
            if (await roleManager.FindByNameAsync(name) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(name));
            }
        }

        public static async Task CreateUserAccounts(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            #region CharityMaker
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string charityMakerEmail = configuration["Data:CharityMaker:Email"];
            string charityMakerPassword = configuration["Data:CharityMaker:Password"];
            string charityMakerRole = configuration["Data:CharityMaker:Role"];

            if (await userManager.FindByEmailAsync(charityMakerEmail) == null)
            {
               
                ApplicationUser user = new ApplicationUser
                {
                    UserName = charityMakerEmail,
                    Email = charityMakerEmail,
                    PersonType = PersonType.User

                };

                user.EmailConfirmed = true;
                IdentityResult result = await userManager.CreateAsync(user, charityMakerPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, charityMakerRole);
                }
            }
            #endregion
            #region Representative
            string representativeEmail = configuration["Data:Representative:Email"];
            string representativePassword = configuration["Data:Representative:Password"];
            string representativeRole = configuration["Data:Representative:Role"];

            if (await userManager.FindByEmailAsync(representativeEmail) == null)
            {

                ApplicationUser user = new ApplicationUser
                {
                    UserName = representativeEmail,
                    Email = representativeEmail,
                    PersonType = PersonType.User

                };

                user.EmailConfirmed = true;
                IdentityResult result = await userManager.CreateAsync(user, representativePassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, representativeRole);
                }
            }
            #endregion
            #region Volunteer
            string volunteerEmail = configuration["Data:Volunteer:Email"];
            string volunteerPassword = configuration["Data:Volunteer:Password"];
            string volunteerRole = configuration["Data:Volunteer:Role"];

            if (await userManager.FindByEmailAsync(volunteerEmail) == null)
            {

                ApplicationUser user = new ApplicationUser
                {
                    UserName = volunteerEmail,
                    Email = volunteerEmail,
                    PersonType = PersonType.User

                };

                user.EmailConfirmed = true;
                IdentityResult result = await userManager.CreateAsync(user, volunteerPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, volunteerRole);
                }
            }
            #endregion
            #region Orphan
            string orphanEmail = configuration["Data:Orphan:Email"];
            string orphanPassword = configuration["Data:Orphan:Password"];
            string orphanRole = configuration["Data:Orphan:Role"];

            if (await userManager.FindByEmailAsync(orphanEmail) == null)
            {

                ApplicationUser user = new ApplicationUser
                {
                    UserName = orphanEmail,
                    Email = orphanEmail,
                    PersonType = PersonType.User

                };

                user.EmailConfirmed = true;
                IdentityResult result = await userManager.CreateAsync(user, orphanPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, orphanRole);
                }
            }
            #endregion
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            #region SoftDeleteSetUp

            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(entity => !entity.IsDeleted);

            #endregion
        }
    }
}

