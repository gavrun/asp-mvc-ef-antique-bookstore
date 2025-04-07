using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Email)
                .HasMaxLength(100) // TODO: Default length for an email 
                .IsRequired(false);

            builder.HasIndex(c => c.Email).IsUnique(); // Email must be unique

            builder.Property(c => c.Phone)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(c => c.IsActive)
                .IsRequired();
                //.HasDefaultValue(true);

            builder.Property(c => c.Comment)
                .HasMaxLength(500)
                .IsRequired(false);

            // Relation to DeliveryAddress (1-to-Many)
            builder.HasMany(c => c.DeliveryAddresses)
                   .WithOne(da => da.Customer)
                   .HasForeignKey(da => da.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade); // При удалении клиента удаляем его адреса

            // Relation to Order (1-to-Many) configured in OrderConfiguration

            // Seed data
            builder.HasData(
                new Customer { Id = 1, FirstName = "Hans", LastName = "Müller", Email = "hans.muller@example.de", IsActive = true },
                new Customer { Id = 2, FirstName = "Sophie", LastName = "Dubois", Email = "sophie.dubois@example.fr", IsActive = true },
                new Customer { Id = 3, FirstName = "Haruki", LastName = "Tanaka", Email = "haruki.tanaka@example.jp", IsActive = true },
                new Customer { Id = 4, FirstName = "Thabo", LastName = "Ndlovu", Email = "thabo.ndlovu@example.za", IsActive = true },
                new Customer { Id = 5, FirstName = "Isabella", LastName = "Fernandez", Email = "isabella.fernandez@example.ar", IsActive = true }
                );
        }
    }
}
