using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AntiqueBookstore.Data.Seed
{
    public static class DataSeeder
    {
        //
        // Data seeding orchestrator
        //

        // Seed all data begins 
        public static async Task SeedDatabaseAsync(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<Program>>(); // Logger for DataSeeder

                var context = services.GetRequiredService<ApplicationDbContext>();

                try
                {
                    logger.LogInformation("Checking and applying migrations...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully.");

                    logger.LogInformation("Starting data seeding...");

                    // Load data by seeding methods
                    await SeedAppUserAsync(context, logger, services); // TODO: finish


                    //Save changes from all seeding methods
                    if (context.ChangeTracker.HasChanges())
                    {
                        logger.LogInformation("Saving changes to the database...");
                        await context.SaveChangesAsync();
                        logger.LogInformation("Data seeding completed successfully.");
                    }
                    else
                    {
                        logger.LogInformation("No changes detected.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred during data seeding.");
                }
            }
        }

        // Creating ApplicationUser
        private static async Task SeedAppUserAsync(ApplicationDbContext context, ILogger logger, IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();


            if (!await context.Employees.AnyAsync() && !await userManager.Users.AnyAsync(u => u.Email.EndsWith("@example.com")))
            {
                logger.LogInformation("Seeding ApplicationUsers and Employees...");

                ApplicationUser managerUser = null;
                ApplicationUser salesUser = null;

                var managerEmail = "manager@example.com";
                var salesEmail = "salesman@example.com";

                // Default Manager user
                if (await userManager.FindByEmailAsync(managerEmail) == null)
                {
                    logger.LogInformation($"User {managerEmail} not found, creating...");
                    managerUser = new ApplicationUser { UserName = managerEmail, Email = managerEmail, EmailConfirmed = true };
                    var result = await userManager.CreateAsync(managerUser, "manager"); // it is TEST password
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(managerUser, "Manager"); 
                        logger.LogInformation($"Created user {managerEmail} with Manager role.");
                    }
                    else 
                    { 
                        logger.LogError($"Failed to create user {managerEmail}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        managerUser = null;
                    }

                }
                else
                {
                    logger.LogInformation($"User {managerEmail} already exists.");
                }

                // TODO: Create link to Employees
                if (managerUser != null)
                {
                    var managerEmployee = await context.Employees.FindAsync(1); // Search by PK
                    if (managerEmployee != null)
                    {
                        if (string.IsNullOrEmpty(managerEmployee.ApplicationUserId))
                        {
                            managerEmployee.ApplicationUserId = managerUser.Id;
                            logger.LogInformation($"Linking Employee ID {managerEmployee.Id} ({managerEmployee.LastName}) to User {managerUser.Email}.");
                        }
                        else if (managerEmployee.ApplicationUserId == managerUser.Id)
                        {
                            logger.LogInformation($"Employee ID {managerEmployee.Id} already linked to User {managerUser.Email}.");
                        }
                        else
                        {
                            logger.LogWarning($"Employee ID {managerEmployee.Id} is already linked to a different user ({managerEmployee.ApplicationUserId}). Cannot link to {managerUser.Email}.");
                        }
                    }
                    else
                    {
                        logger.LogWarning($"Employee with ID 1 (expected Manager) not found in the database. Cannot link user {managerUser.Email}.");
                    }
                }

                // Default Salesman user
                if (await userManager.FindByEmailAsync(salesEmail) == null)
                {
                    logger.LogInformation($"User {salesEmail} not found, creating...");
                    salesUser = new ApplicationUser { UserName = salesEmail, Email = salesEmail, EmailConfirmed = true };
                    var result = await userManager.CreateAsync(salesUser, "salesman"); // it is TEST password
                    if (result.Succeeded) 
                    { 
                        await userManager.AddToRoleAsync(salesUser, "Sales"); 
                        logger.LogInformation($"Created user {salesEmail} with Sales role."); 
                    }
                    else 
                    { 
                        logger.LogError($"Failed to create user {salesEmail}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        salesUser = null;
                    }
                }
                else
                {
                    logger.LogInformation($"User {managerEmail} already exists.");
                }

                // TODO: Create link to Employees
                if (salesUser != null)
                {
                    var salesEmployee = await context.Employees.FindAsync(2); // Search by PK
                    if (salesEmployee != null)
                    {
                        if (string.IsNullOrEmpty(salesEmployee.ApplicationUserId))
                        {
                            salesEmployee.ApplicationUserId = salesUser.Id;
                            logger.LogInformation($"Linking Employee ID {salesEmployee.Id} ({salesEmployee.LastName}) to User {salesUser.Email}.");
                        }
                        else if (salesEmployee.ApplicationUserId == salesUser.Id)
                        {
                            logger.LogInformation($"Employee ID {salesEmployee.Id} already linked to User {salesUser.Email}.");
                        }
                        else
                        {
                            logger.LogWarning($"Employee ID {salesEmployee.Id} is already linked to a different user ({salesEmployee.ApplicationUserId}). Cannot link to {salesUser.Email}.");
                        }
                    }
                    else
                    {
                        logger.LogWarning($"Employee with ID 2 (expected Sales) not found in the database. Cannot link user {salesUser.Email}.");
                    }
                }
            }  
        }

        // End of data seeding
    }
}
