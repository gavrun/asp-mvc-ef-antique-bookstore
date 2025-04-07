using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;

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
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                try
                {
                    // Check and apply migrations
                    logger.LogInformation("Applying migrations...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully.");

                    // Seed user
                    logger.LogInformation("Starting seeding...");
                    await SeedRolesAsync(roleManager, logger);
                    await SeedUsersAsync(userManager, context, logger);
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"An error occurred during seeding: {ex.Message}");
                }
            }
        }

        


        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            // Create roles in Identity
            string[] roleNames = { "Manager", "Sales" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger logger)
        {
            var adminUser = new ApplicationUser { UserName = "manager@example.com", Email = "manager@example.com", EmailConfirmed = true };
            var salesUser = new ApplicationUser { UserName = "sales@example.com", Email = "sales@example.com", EmailConfirmed = true };

            // Creating 
            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                logger.LogInformation("Creating user...");
                await userManager.CreateAsync(adminUser, "manager");
                await userManager.AddToRoleAsync(adminUser, "Manager");
                logger.LogInformation("User created successfully");
            }

            if (await userManager.FindByEmailAsync(salesUser.Email) == null)
            {
                logger.LogInformation("Creating user...");
                await userManager.CreateAsync(salesUser, "sales");
                await userManager.AddToRoleAsync(salesUser, "Sales");
                logger.LogInformation("User created successfully");
            }

            // Linking
            var manager = await userManager.FindByEmailAsync("manager@example.com");
            var sales = await userManager.FindByEmailAsync("sales@example.com");

            var employees = await context.Employees
                .Include(e => e.PositionHistories)
                .ThenInclude(ph => ph.Position)
                .ToListAsync();

            var managerEmployee = employees
                .FirstOrDefault(e => e.PositionHistories
                .Any(ph => ph.Position.LevelId == 1 && ph.IsActive));
            var salesEmployee = employees
                .FirstOrDefault(e => e.PositionHistories
                .Any(ph => ph.Position.LevelId == 2 && ph.IsActive));

            if (manager != null && managerEmployee != null)
            {
                manager.EmployeeId = managerEmployee.Id;
                await userManager.UpdateAsync(manager);
            }

            if (sales != null && salesEmployee != null)
            {
                sales.EmployeeId = salesEmployee.Id;
                await userManager.UpdateAsync(sales);
            }
        }


        private static async Task SeedAppUserAsync1(ApplicationDbContext context, ILogger logger, IServiceProvider services)
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
