using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace WebAppTest1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // IdentityDbContext<ApplicationUser> to hook up IdentityUser
    {
        public DbSet<Book> Books { get; set; } 
        public DbSet<Author> Authors { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Role> Roles { get; set; } // User entity domain Role
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleEvent> SaleEvents { get; set; }

        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<PositionHistory> PositionHistories { get; set; }

        public DbSet<BookCondition> BookConditions { get; set; }
        public DbSet<BookStatus> BookStatuses { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<SalesAuditLog> SalesAuditLogs { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
