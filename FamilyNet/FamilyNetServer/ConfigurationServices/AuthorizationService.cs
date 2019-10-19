using FamilyNetServer.Configuration;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace FamilyNetServer.ConfigurationServices
{
    public static class AuthorizationService
    {
        public static void AddAuthorizationService(this IServiceCollection services,
                                                   IConfiguration configuration)
        {
            var JWTConfigurationSection = configuration.GetSection("JWT");
            services.Configure<JWTCofiguration>(JWTConfigurationSection);

            var JWTConfiguration = JWTConfigurationSection.Get<JWTCofiguration>();
            var key = Encoding.ASCII.GetBytes(JWTConfiguration.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userManager = context.HttpContext.RequestServices
                            .GetRequiredService<IUnitOfWork>().UserManager;
                        var name = context.Principal.Identity.Name;
                        var profile = userManager.FindByNameAsync(name);

                        if (profile == null)
                        {
                            context.Fail("Unauthorized");
                        }

                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}
