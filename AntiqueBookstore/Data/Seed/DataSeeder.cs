using AntiqueBookstore.Domain.Entities;
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

                //var logger = services.GetRequiredService<ILogger<DataSeeder>>(); // BUG: Logger for DataSeeder
                var logger = services.GetRequiredService<ILogger<Program>>();

                var context = services.GetRequiredService<ApplicationDbContext>();

                try
                {
                    logger.LogInformation("Checking and applying migrations...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully.");

                    logger.LogInformation("Starting data seeding...");

                    // Load data by seeding methods
                    await SeedEmployeesAsync(context, logger);
                    await SeedBookCatalogAsync(context, logger);
                    await SeedCustomersAsync(context, logger);
                    await SeedOrdersAsync(context, logger);


                    // Save changes from all seeding methods
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

        // Seed employees data
        private static async Task SeedEmployeesAsync(ApplicationDbContext context, ILogger logger)
        {
            // TODO: Seed test employees data
            await Task.CompletedTask;
        }

        // Seed book catalog data
        private static async Task SeedBookCatalogAsync(ApplicationDbContext context, ILogger logger)
        {
            // TODO: Seed test book catalog data
            await Task.CompletedTask;
        }

        // Seed customers data
        private static async Task SeedCustomersAsync(ApplicationDbContext context, ILogger logger)
        {
            // TODO: Seed test customers data
            await Task.CompletedTask;
        }

        // Seed orders data
        private static async Task SeedOrdersAsync(ApplicationDbContext context, ILogger logger)
        {
            // TODO: Seed test orders data
            await Task.CompletedTask;
        }

        // SaveChanges will happen in SeedDatabaseAsync
        // End of seeding
    }
}
