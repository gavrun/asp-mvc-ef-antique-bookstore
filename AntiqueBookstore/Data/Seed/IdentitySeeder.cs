using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AntiqueBookstore.Data.Seed
{
    public static class IdentitySeeder
    {

        public static async Task SeedUserAsync(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>(); // Logger
                var context = services.GetRequiredService<ApplicationDbContext>();

                try
                {
                    // Check and apply migrations
                    logger.LogInformation("Applying migrations...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully.");

                    // Seed user
                    logger.LogInformation("Starting seeding...");
                    await SeedAppUserAsync(context, logger, services);
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"An error occurred during data seeding: {ex.Message}");
                }
            }
        }

        private static async Task SeedAppUserAsync(ApplicationDbContext context, ILogger logger, IServiceProvider services)
        {
            // Get UserManager from the service provider
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            ApplicationUser managerUser = null;
            ApplicationUser salesUser = null;
            // example { UserName = managerEmail, Email = managerEmail, EmailConfirmed = true }
            var managerEmail = "manager@example.com";
            var salesEmail = "salesman@example.com";
            
            // manager
            if (await userManager.FindByEmailAsync(managerEmail) == null)
            {
                // Create a new user 
                managerUser = new ApplicationUser { UserName = managerEmail, Email = managerEmail };
                // Save the user to the database
                logger.LogInformation("Creating user...");
                await userManager.CreateAsync(managerUser, "manager");
                logger.LogInformation("User created successfully");
            }
            else
            {
                logger.LogInformation($"User {managerEmail} already exists.");
            }

            // UserManager.CreateAsync similar to SaveChangesAsync

            // TODO: Create link to Employees
            if (managerUser != null)
            {
                var managerEmployee = await context.Employees.FindAsync(1);
                if (managerEmployee == null)
                {
                    if (string.IsNullOrEmpty(managerEmployee.ApplicationUserId))
                    {
                        logger.LogWarning("Linking Employee..");
                        managerEmployee.ApplicationUserId = managerUser.Id;
                        logger.LogWarning("Employee linked successfully");
                    }
                    else
                    {
                        logger.LogWarning("Employee is already linked");
                    }
                }
            }
            

            // sales
            if (await userManager.FindByEmailAsync(salesEmail) == null)
            {
                salesUser = new ApplicationUser { UserName = salesEmail, Email = salesEmail };

                await userManager.CreateAsync(salesUser, "salesman");
            }

            if (salesUser != null)
            {
                var salesEmployee = await context.Employees.FindAsync(1);
                if (salesEmployee == null)
                {
                    if (string.IsNullOrEmpty(salesEmployee.ApplicationUserId))
                    {
                        salesEmployee.ApplicationUserId = salesUser.Id;
                    }
                }
            }

        }

    }
}
