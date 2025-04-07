using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role> 
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.Name).IsUnique();

            builder.Property(r => r.Description)
                .HasMaxLength(255);

            builder.Property(r => r.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Seed Roles 'Manager, Sales'
            builder.HasData(
                new Role { Id = 1, Name = "Administrator", Description = "Full system access", IsActive = false },
                // Administrator Role only can delete records from DB in UI, controls in UI not implemented
                // No Role can delete records from SalesAuditLog
                new Role { Id = 2, Name = "Manager", Description = "Manages store operations", IsActive = true },
                new Role { Id = 3, Name = "Sales", Description = "Handles sales and customer interactions", IsActive = true }
            );

            // TODO: Link to Position
        }
    }
}
