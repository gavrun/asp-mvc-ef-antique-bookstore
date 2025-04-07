using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class LevelConfiguration : IEntityTypeConfiguration<Level> 
    {
        public void Configure(EntityTypeBuilder<Level> builder)
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
                new Level { Id = 1, Name = "Manager", Description = "Manages store operations", IsActive = true },
                new Level { Id = 2, Name = "Sales", Description = "Handles sales and customer interactions", IsActive = true }
            );

            // TODO: Link to Position
        }
    }
}
