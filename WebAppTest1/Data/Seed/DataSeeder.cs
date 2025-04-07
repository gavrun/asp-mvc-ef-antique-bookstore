using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAppTest1.Data;

namespace AntiqueBookstore.Data.Seed
{
    public static class DataSeeder
    {
 
        public static async Task SeedDatabaseAsync(IHost host)
        {
            
        }

        
        private static async Task SeedAppUserAsync(ApplicationDbContext context, ILogger logger, IServiceProvider services)
        {
            // Create ApplicationUser and put it in the Identity database
        }

    }
}
