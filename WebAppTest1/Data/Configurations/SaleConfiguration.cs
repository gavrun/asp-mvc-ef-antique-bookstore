using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.SalePrice)
                   .HasPrecision(9, 2)
                   .IsRequired();

            // Relation to Order configured in OrderConfiguration (WithOne)

            // Relation to Books
            builder.HasOne(s => s.Book)
                   .WithMany(b => b.Sales) // Navigation property in Books
                   .HasForeignKey(s => s.BookId)
                   .OnDelete(DeleteBehavior.Restrict); // Cannot delete a Books if it was sold

            // Relation to SaleEvent (Nullable, FK)
            builder.HasOne(s => s.SaleEvent)
                   .WithMany(se => se.Sales) // Navigation property in SaleEvent
                   .HasForeignKey(s => s.EventId)
                   .IsRequired(false) // Not required to attach a SaleEvent
                   .OnDelete(DeleteBehavior.SetNull); // Reset the FK on Sale if the stock item is removed

            // Seed data
            builder.HasData(
                new Sale { Id = 1, SalePrice = 750.00m, OrderId = 1, BookId = 1 }
                );
        }
    }
}
