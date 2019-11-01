using FamilyNetServer.Models.EntityFramework;
using FamilyNetServer.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyNetServer.ConfigurationServices
{
    public static class DataBaseContextService
    {
        public static void AddDBContextService(this IServiceCollection services,
                                               IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration["Data:FamilyNet:ConnectionString"]));
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseSqlServer(configuration["Data:FamilyNetIdentity:ConnectionString"]));
        }
    }
}
