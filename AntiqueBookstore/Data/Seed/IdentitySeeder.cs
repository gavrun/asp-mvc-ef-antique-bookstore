using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;

namespace AntiqueBookstore.Data.Seed
{
    public static class IdentitySeeder
    {
        // Seeding static data for Identity 
        public static async Task SeedUserAsync(IHost host)
        {
            using (var scope = host.Services.CreateScope()) // visibility to resolve scoped services
            {
                // Get services from the scope
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>(); // Logger
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>(); 
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                try
                {
                    // Check waiting and apply migrations
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
            var unlinkedUser = new ApplicationUser { UserName = "unlinked@example.com", Email = "unlinked@example.com", EmailConfirmed = true }; 

            // Creating users
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

            if (await userManager.FindByEmailAsync(unlinkedUser.Email) == null)
            {
                logger.LogInformation("Creating user...");
                await userManager.CreateAsync(unlinkedUser, "unlinked");
                //await userManager.AddToRoleAsync(unlinkedUser, "Sales");
                logger.LogInformation("User created successfully");
            }

            // Linking users to employees
            var manager = await userManager.FindByEmailAsync("manager@example.com");
            var sales = await userManager.FindByEmailAsync("sales@example.com");
            //var unlinked = await userManager.FindByEmailAsync("unlinked@example.com");

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

    }
}
