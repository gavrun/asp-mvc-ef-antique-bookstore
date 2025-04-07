using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class SaleEventConfiguration : IEntityTypeConfiguration<SaleEvent>
    {
        public void Configure(EntityTypeBuilder<SaleEvent> builder)
        {
            builder.HasKey(se => se.Id);

            builder.Property(se => se.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(se => se.Discount)
                   .HasPrecision(4, 2) // Precision for Discount, for example, from 0.00 to 0.99 (1.00)
                   .IsRequired();

            builder.Property(se => se.StartDate)
                .IsRequired();

            builder.Property(se => se.EndDate)
                .IsRequired();


            // Relation to Sale (1-to-Many) configured in SaleConfiguration (WithOne)
        }
    }
}
