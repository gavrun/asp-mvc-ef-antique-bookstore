using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class BookStatusConfiguration : IEntityTypeConfiguration<BookStatus>
    {
        public void Configure(EntityTypeBuilder<BookStatus> builder)
        {
            builder.HasKey(bs => bs.Id);

            builder.Property(bs => bs.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(bs => bs.Name).IsUnique();

            builder.Property(bs => bs.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasData(
                new BookStatus { Id = 1, Name = "Available", IsActive = true },
                new BookStatus { Id = 2, Name = "Reserved", IsActive = true },
                new BookStatus { Id = 3, Name = "Sold", IsActive = true },
                new BookStatus { Id = 4, Name = "Archived", IsActive = true } // For example, for deleted or decommissioned books
            );
        }
    }
}
