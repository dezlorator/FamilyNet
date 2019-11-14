using FamilyNetLogs.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyNetLogs.ApplicationBuilderExtension
{
    public static class ApplicationBuilderAutoGenerationDataBase
    {
        public static void AutoGenerationDataBase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider
                    .GetRequiredService<FamilyNetLogsContext>();

                context.Database.Migrate();
            }
        }
    }
}
