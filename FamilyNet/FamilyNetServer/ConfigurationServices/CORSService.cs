using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyNetServer.ConfigurationServices
{
    public static class CORSService
    {
        public static void CORS(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigin", builder => builder
                    .AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllOrigin"));
            });
        }
    }
}
