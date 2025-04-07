using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class BookConditionConfiguration : IEntityTypeConfiguration<BookCondition> // used when ApplyConfigurationsFromAssembly
    {
        // configure the BookCondition entity
        public void Configure(EntityTypeBuilder<BookCondition> builder)
        {
            // Set table name
            //builder.ToTable("BookConditions");

            // Set primary key
            builder.HasKey(bc => bc.Id);

            // Set properties
            builder.Property(bc => bc.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Set indexes
            builder.HasIndex(bc => bc.Name).IsUnique();

            builder.Property(bc => bc.Description)
                .HasMaxLength(255);

            builder.Property(bc => bc.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Set relationships
            //builder.HasMany(bc => bc.Books)
            //    .WithOne(b => b.BookCondition)
            //    .HasForeignKey(b => b.BookConditionId);

            // Seed data
            builder.HasData(
                new BookCondition { Id = 1, Name = "Excellent", Description = "As new", IsActive = true },
                new BookCondition { Id = 2, Name = "Very Good", Description = "Minor signs of wear", IsActive = true },
                new BookCondition { Id = 3, Name = "Good", Description = "Average wear", IsActive = true },
                new BookCondition { Id = 4, Name = "Fair", Description = "Significant wear", IsActive = true },
                new BookCondition { Id = 5, Name = "Poor", Description = "Damaged", IsActive = true }
            );
        }
    }
}
