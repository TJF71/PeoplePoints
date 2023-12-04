using contactPro2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace contactPro2.Data
{
    public static class DataUtility
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");



            return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<AppUser>>();
            var configurationsSvc = svcProvider.GetRequiredService<IConfiguration>();

            // Align the datbase by checking the Migrations
            await dbContextSvc.Database.MigrateAsync();

            // Seed some info!
            await SeedDemoUsersAsync(userManagerSvc, configurationsSvc);

        }
        // Demo Users Seed Method
        private static async Task SeedDemoUsersAsync(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            string? demoLoginEmail = configuration["DemoLoginEmail"] ?? Environment.GetEnvironmentVariable("DemoLoginEmail");
            string? demoLoginPassword = configuration["DemoLoginPassword"] ?? Environment.GetEnvironmentVariable("DemoLoginPassword");

            AppUser demoUser = new AppUser()
            {
                UserName = demoLoginEmail,
                Email = demoLoginEmail,
                FirstName = "Demo",
                LastName = "User",
                EmailConfirmed = true
            };

            try
            {
                AppUser? appUser = await userManager.FindByEmailAsync(demoLoginEmail!);

                if (appUser == null)
                {
                    await userManager.CreateAsync(demoUser, demoLoginPassword!);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Login User.");
                Console.WriteLine(exception.Message);
                Console.WriteLine("***********************************");

                throw;
            }

        }

    }



}
