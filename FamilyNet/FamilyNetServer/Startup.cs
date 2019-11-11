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
using Microsoft.Extensions.Logging;
using FamilyNetServer.Controllers.API.V1;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using FamilyNetServer.Controllers.API;
using FamilyNetServer.EnumConvertor;
using FamilyNetServer.HttpHandlers;

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
            services.AddTransient<IIdentityExtractor, IdentityExtractor>();
            services.AddTransient<IPasswordValidator<ApplicationUser>, FamilyNetServerPasswordValidator>();
            services.AddTransient<IUserValidator<ApplicationUser>, FamilyNetServerUserValidator>();
            services.AddDBContextService(Configuration);
            services.AddIdentityService();
            services.AddTransient<ITokenFactory, TokenFactory>();
            services.AddAuthorizationService(Configuration);
            services.AddTransient<IConvertUserRole, ConvertUserRole>();
            services.AddTransient<ILogger<FioController>, Logger<FioController>>();
            services.Configure<ServerURLSettings>(Configuration.GetSection("Server"));
            services.AddTransient<EFRepository<Feedback>, FeedbackRepository>();
            services.AddTransient<ILogger<FeedbackController>, Logger<FeedbackController>>();
            services.AddTransient<IFeedbackValidator, FeedbackValidator>();
            services.AddTransient<IUnitOfWork, EFUnitOfWork>();
            services.AddTransient<ILogger<CharityMakersController>, Logger<CharityMakersController>>();
            services.AddTransient<ILogger<ChildrenController>, Logger<ChildrenController>>();
            services.AddTransient<ILogger<VolunteersController>, Logger<VolunteersController>>();
            services.AddTransient<ILogger<AddressController>, Logger<AddressController>>();
            services.AddTransient<ILogger<ChildrenHouseController>, Logger<ChildrenHouseController>>();
            services.AddTransient<ILogger<LocationController>, Logger<LocationController>>();
            services.AddTransient<ILogger<AuctionLotController>, Logger<AuctionLotController>>();
            services.AddTransient<ILogger<PurchaseController>, Logger<PurchaseController>>();
            services.AddTransient<ILogger<QuestsController>, Logger<QuestsController>>();
            services.AddTransient<IFileUploader, FileUploader>();
            services.AddTransient<IChildValidator, ChildValidator>();
            services.AddTransient<IVolunteerValidator, VolunteerValidator>();
            services.AddTransient<ICharityMakerValidator, CharityMakerValidator>();
            services.AddTransient<ICharityMakersSelection, CharityMakersSelection>();
            services.AddTransient<IFilterConditionsVolunteers, FilterConditionsVolunteers>();
            services.AddTransient<IFilterConditionsChildren, FilterConditionsChildren>();
            services.AddTransient<IRepresentativeValidator, RepresentativeValidator>();
            services.AddTransient<IFilterConditionsRepresentatives, FilterConditionsRepresentatives>();
            services.AddTransient<IFilterConditionsChildrenHouse, FilterConditionChildrenHouse>();
            services.Configure<ServerURLSettings>(Configuration.GetSection("Server"));
            services.AddTransient<IValidator<AddressDTO>, AddressValidator>();
            services.AddTransient<IValidator<AuctionLotDTO>, AuctionLotValidator>();
            services.AddTransient<IValidator<ChildrenHouseDTO>, ChildrenHouseValidator>();
            services.AddTransient<IValidator<PurchaseDTO>, PurchaseValidator>();
            services.AddTransient<IValidator<CategoryDTO>, CategoryValidator>();
            services.AddTransient<IValidator<DonationItemDTO>, DonationItemValidator>();
            services.AddTransient<IValidator<DonationDTO>, DonationValidator>();
            services.AddTransient<IDonationsFilter, DonationsFilter>();
            services.AddTransient<IValidator<QuestDTO>, QuestValidator>();
            services.AddTransient<IQuestsFilter, QuestsFilter>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigin", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllOrigin"));
            });
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
            //services.AddApiVersioning(o => o.ApiVersionReader = new HeaderApiVersionReader("api-version"));
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
