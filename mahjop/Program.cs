using mahjop.Data;
using mahjop.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mahjop
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
          var host=  CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerProvider>();
            var logger = loggerFactory.CreateLogger("app");

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>(); // استخراج الـ AppContext من خدمات الـ DI
                await Seeds.DefaultMedicationCategory.SeedAsync(context); // إضافة بيانات الفئات الافتراضية

                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await Seeds.DefaultRoles.SeedAsync(roleManager);
                await Seeds.DefaultUsers.SeedBasivUserAsync(userManager);
                await Seeds.DefaultUsers.SeedAdminAsync(userManager);
                // await Seeds.DefaultUsers.SeedAdminUserAsync(userManager);
                await Seeds.DefaultUsers.SeedSuperAdminUserAsync(userManager,roleManager);
                logger.LogInformation("Data seeded");
                logger.LogInformation("Application Stated");
            }
            catch (Exception ex)
            {

                logger.LogWarning(ex, "An error occurred wiile seeding data");
            }
                host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
