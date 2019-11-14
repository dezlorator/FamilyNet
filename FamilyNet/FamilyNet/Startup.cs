using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using FamilyNet.Configuration;
using FamilyNet.Downloader;
using DataTransferObjects;
using FamilyNet.StreamCreater;
using FamilyNet.HttpHandlers;
using FamilyNet.IdentityHelpers;
using FamilyNet.Downloader.URLBuilders;
using System;
using Microsoft.AspNetCore.Http;
using FamilyNet.Downloader.Interfaces;

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
            services.AddTransient<IAuthorizeCreater, AuthorizeCreater>();           
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            #region URLBuildres

            services.AddTransient<IURLVolunteersBuilder, URLVolunteersBuilder>();
            services.AddTransient<IURLDonationsBuilder, URLDonationsBuilder>();
            services.AddTransient<IURLDonationItemsBuilder, URLDonationItemsBuilder>();
            services.AddTransient<IURLCategoriesBuilder, URLCategoriesBuilder>();
            services.AddTransient<IURLAuctionLotBuilder, URLAuctionLotBuilder>();
            services.AddTransient<IURLLocationBuilder, URLLocationBuilder>();
            services.AddTransient<IURLChildrenHouseBuilder, URLChildrenHouseBuilder>();
            services.AddTransient<IURLAddressBuilder, URLAddressBuilder>();
            services.AddTransient<IURLRepresentativeBuilder, URLRepresentativesBuilder>();
            services.AddTransient<IURLVolunteersBuilder, URLVolunteersBuilder>();
            services.AddTransient<IURLCharityMakerBuilder, URLCharityMakerBuilder>();
            services.AddTransient<IURLChildrenBuilder, URLChildrenBuilder>();
            services.AddTransient<IURLPurchaseBuilder, URLPurchaseBuilder>();
            services.AddTransient<IURLRegistrationBuilder, URLRegistrationBuilder>();
            services.AddTransient<IURLUsersBuilder, URLUsersBuilder>();
            services.AddTransient<IURLRolesBuilder, URLRolesBuilder>();
            services.AddTransient<IURLChildrenActivitesBuilder, URLChildrenActivitiesBuilder>();

            #endregion

            #region ServerDataDownloader

            services.AddTransient<IServerRepresenativesDataDownloader, ServerRepresentativesDownloader>();
            services.AddTransient<IFioDownloader, ServerFioDownloader>();
            services.AddTransient<IURLFioBuilder, URLFioBuilder>();
            services.AddTransient<ServerDataDownloader<ChildDTO>, ServerChildrenDownloader>();
            services.AddTransient<ServerDataDownloader<CharityMakerDTO>, ServerCharityMakersDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<DonationDetailDTO>, ServerDonationsDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<DonationItemDTO>, ServerDonationItemsDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<CategoryDTO>, ServerCategoriesDownloader>();
            services.AddTransient<ServerDataDownloader<ChildrenHouseDTO>, ServerChildrenHouseDownloader>();
            services.AddTransient<ServerChildrenHouseDownloader>();
            services.AddTransient<ServerDataDownloader<RepresentativeDTO>, ServerRepresentativesDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<RoleDTO>, ServerRoleDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<UserDTO>, ServerUserDownloader>();
            services.AddTransient<IServerAvailabilitiesDownloader, ServerAvailabilitiesDownloader>();
            services.AddTransient<IServerAddressDownloader, ServerAddressDownloader>();
            services.AddTransient<IURLChildrenBuilder, URLChildrenBuilder>();
            services.AddTransient<IURLAvailabilitiesBuilder, URLAvailabilitiesBuilder>();

            services.AddTransient<ServerChildrenHouseDownloader>();
            services.AddTransient<ServerAddressDownloader>();
            services.AddTransient<ServerLocationDownloader>();
            services.AddTransient<ServerDataDownloader<FeedbackDTO>, ServerFeedbackDownloader>();
            services.AddTransient<ServerDataDownloader<CharityMakerDTO>, ServerCharityMakersDownloader>();
            services.AddTransient<ServerDataDownloader<AuctionLotDTO>, ServerAuctionLotDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<RegistrationDTO>, ServerRegistrationDownloader>();
            services.AddTransient<IURLFeedbackBuilder, URLFeedbackBuilder>();
            services.AddTransient<IURLAuctionLotBuilder, URLAuctionLotBuilder>();
            services.AddTransient<IURLLocationBuilder, URLLocationBuilder>();
            services.AddTransient<IURLChildrenHouseBuilder, URLChildrenHouseBuilder>();
            services.AddTransient<IURLAddressBuilder, URLAddressBuilder>();
            services.AddTransient<IURLRepresentativeBuilder, URLRepresentativesBuilder>();
            services.AddTransient<IURLVolunteersBuilder, URLVolunteersBuilder>();
            services.AddTransient<IURLCharityMakerBuilder, URLCharityMakerBuilder>();
            services.AddTransient<IIdentityInformationExtractor, IdentityInformationExtractor>();
            services.AddTransient<IURLQuestsBuilder, URLQuestsBuilder>();
            services.AddTransient<ServerDataDownloader<VolunteerDTO>, ServerVolunteersDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<PurchaseDTO>, ServerPurchaseDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<QuestDTO>, ServerQuestsDownloader>();
            services.AddTransient<ServerSimpleDataDownloader<ChildActivityDTO>, ServerChildrenActivitiesDownloader>();

            #endregion

            services.Configure<ServerURLSettings>(Configuration.GetSection("Server"));            
            services.AddTransient<IServerAddressDownloader, ServerAddressDownloader>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddTransient<IHttpAuthorizationHandler, HttpAuthorizationHandler>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddViewLocalization(
                Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
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
            app.UseSession();
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
