using FamilyNet.Models.EntityFramework;
using FamilyNet.Models.Interfaces;
using FamilyNet.Models.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using FamilyNet.Infrastructure;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using FamilyNet.Configuration;
using FamilyNet.Downloader;
using DataTransferObjects;
using FamilyNet.StreamCreater;

namespace FamilyNet
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
            services.AddTransient<IFileStreamCreater, FileStreamCreater>();
            services.AddTransient<IPasswordValidator<ApplicationUser>, FamilyNetPasswordValidator>();
            services.AddTransient<IUserValidator<ApplicationUser>, FamilyNetUserValidator>();
            //services.AddTransient<FamilyNetPhoneValidator>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration["Data:FamilyNet:ConnectionString"]));
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseSqlServer(Configuration["Data:FamilyNetIdentity:ConnectionString"]));
            services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireDigit = true;

            }).AddEntityFrameworkStores<ApplicationIdentityDbContext>()
            .AddUserManager<ApplicationUserManager>()
            .AddDefaultTokenProviders();

            services.Configure<ServerURLSettings>(Configuration.GetSection("Server"));
            services.AddTransient<ServerDataDownLoader<ChildDTO>, ServerChildrenDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<DonationDetailDTO>, ServerDonationsDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<DonationItemDTO>, ServerDonationItemsDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<CategoryDTO>, ServerCategoriesDownloader>();
            services.AddTransient<IURLChildrenBuilder, URLChildrenBuilder>();
            services.AddTransient<IURLDonationsBuilder, URLDonationsBuilder>();
            services.AddTransient<IURLDonationItemsBuilder, URLDonationItemsBuilder>();
            services.AddTransient<IURLCategoriesBuilder, URLCategoriesBuilder>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddTransient<IUnitOfWorkAsync, EFUnitOfWorkAsync>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddViewLocalization(
                Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
            //    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default2",
                    template: "{controller}/{action}/{id?}"
                    );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
