using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AntiqueBookstore.Data.Seed
{
    public static class IdentitySeeder
    {

        public static async Task SeedUserAsync(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                try
                {
                    await context.Database.MigrateAsync();

                    await SeedAppUserAsync(services);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred during data seeding: {ex.Message}");
                }
            }
        }

        private static async Task SeedAppUserAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var managerEmail = "manager@example.com";
            var salesEmail = "salesman@example.com";
            //{ UserName = managerEmail, Email = managerEmail, EmailConfirmed = true }

            if (await userManager.FindByEmailAsync(managerEmail) == null)
            {
                var managerUser = new ApplicationUser { UserName = managerEmail, Email = managerEmail };

                await userManager.CreateAsync(managerUser, "manager");
            }

            if (await userManager.FindByEmailAsync(salesEmail) == null)
            {
                var salesUser = new ApplicationUser { UserName = salesEmail, Email = salesEmail };

                await userManager.CreateAsync(salesUser, "salesman");
            }

            // UserManager.CreateAsync instead of SaveChangesAsync
        }

    }
}
