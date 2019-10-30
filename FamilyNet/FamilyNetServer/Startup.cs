using FamilyNetServer.Models;
using FamilyNetServer.Models.EntityFramework;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Models.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using FamilyNetServer.Infrastructure;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using FamilyNetServer.Validators;
using FamilyNetServer.Filters;
using FamilyNetServer.Uploaders;
using FamilyNetServer.ConfigurationServices;
using FamilyNetServer.Configuration;
using FamilyNetServer.Factories;
using DataTransferObjects;
using NLog;
using Microsoft.Extensions.Logging;
using FamilyNetServer.Controllers.API;

namespace FamilyNetServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPasswordValidator<ApplicationUser>, FamilyNetServerPasswordValidator>();
            services.AddTransient<IUserValidator<ApplicationUser>, FamilyNetServerUserValidator>();
            services.AddDBContextService(Configuration);
            services.AddIdentityService();
            services.AddTransient<ITokenFactory, TokenFactory>();
            services.AddAuthorizationService(Configuration);
            services.Configure<ServerURLSettings>(Configuration.GetSection("Server"));
            services.AddTransient<IUnitOfWork, EFUnitOfWork>();
            services.AddTransient<ILogger<CharityMakersController>, Logger<CharityMakersController>>();
            services.AddTransient<ILogger<VolunteersController>, Logger<VolunteersController>>();
            services.AddTransient<IFileUploader, FileUploader>();
            services.AddTransient<IChildValidator, ChildValidator>();
            services.AddTransient<IVolunteerValidator, VolunteerValidator>();
            services.AddTransient<ICharityMakerValidator, CharityMakerValidator>();
            services.AddTransient<ICharityMakersSelection, CharityMakersSelection>();
            services.AddTransient<IFilterConditionsVolunteers, FilterConditionsVolunteers>();
            services.AddTransient<IFilterConditionsChildren, FilterConditionsChildren>();
            services.AddTransient<IRepresentativeValidator, RepresentativeValidator>();
            services.AddTransient<IFilterConditionsRepresentatives, FilterConditionsRepresentatives>();
            services.AddTransient<IDonationsFilter, DonationsFilter>();
            services.AddTransient<IFilterConditionsChildrenHouse, FilterConditionChildrenHouse>();
            services.Configure<ServerURLSettings>(Configuration.GetSection("Server"));
            services.AddTransient<IValidator<AddressDTO>, AddressValidator>();
            services.AddTransient<IValidator<ChildrenHouseDTO>, ChildrenHouseValidator>();
            services.AddTransient<ICategoryValidator, CategoryValidator>();
            services.AddTransient<IDonationItemValidator, DonationItemValidator>();
            services.AddTransient<IDonationValidator, DonationValidator>();
            services.AddTransient<IDonationItemsFilter, DonationItemsFilter>();
            services.AddTransient<IDonationsFilter, DonationsFilter>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddViewLocalization(
                Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .ConfigureApiBehaviorOptions(options =>
             {
                 options.SuppressConsumesConstraintForFormFileParameters = true;
                 options.SuppressInferBindingSourcesForParameters = true;
                 options.SuppressModelStateInvalidFilter = true;
                 options.SuppressMapClientErrors = true;
                 options.ClientErrorMapping[404].Link =
                     "https://httpstatuses.com/404";
             });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/localization?view=aspnetcore-2.2
            var supportedCultures = new[]
            {
                new CultureInfo("uk-UA"),
                //new CultureInfo("en-US"),
                new CultureInfo("ru-RU")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("uk-UA"),
                //Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                //UI strings that we have localized
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();

            //ApplicationIdentityDbContext.CreateAdminAccount(app.ApplicationServices,
            //        Configuration).Wait();
            //ApplicationIdentityDbContext.InitializeRolesAsync(app.ApplicationServices).Wait();
        }
    }
}
