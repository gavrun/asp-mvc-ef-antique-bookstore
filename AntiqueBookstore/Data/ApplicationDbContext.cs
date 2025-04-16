using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;

namespace AntiqueBookstore.Data
{
    //public class ApplicationDbContext : IdentityDbContext
    //{
    //    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    //        : base(options)
    //    {
    //    }
    //}

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // hook up <ApplicationUser> (IdentityUser) to IdentityDbContext
    {
        // DbSet for main model entities
        public DbSet<Book> Books { get; set; } 
        public DbSet<Author> Authors { get; set; }
        public virtual DbSet<Employee> Employees { get; set; } // Moq lib needs virtual
        public DbSet<Position> Positions { get; set; }
        public DbSet<Level> Levels { get; set; } // User entity Level
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleEvent> SaleEvents { get; set; }

        // DbSet for other linking entities
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<PositionHistory> PositionHistories { get; set; }

        // DbSet for referencing entities
        public DbSet<BookCondition> BookConditions { get; set; }
        public DbSet<BookStatus> BookStatuses { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        // audit log for Sales
        public DbSet<SalesAuditLog> SalesAuditLogs { get; set; }


        // constructor with config options
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        // for Fluent API configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Applly default Identity configuration for Users, Roles, etc.
            base.OnModelCreating(modelBuilder);

            // Register all IEntityTypeConfiguration<TEntity> from the current assembly

            // Apply all configurations 

            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
