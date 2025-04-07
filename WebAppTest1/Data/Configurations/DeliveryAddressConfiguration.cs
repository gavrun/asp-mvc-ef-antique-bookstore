using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class DeliveryAddressConfiguration : IEntityTypeConfiguration<DeliveryAddress>
    {
        public void Configure(EntityTypeBuilder<DeliveryAddress> builder)
        {
            builder.HasKey(da => da.Id);

            builder.Property(da => da.AddressAlias)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(da => da.Country)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(da => da.City)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(da => da.AddressLine1)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(da => da.AddressLine2)
               .HasMaxLength(255)
               .IsRequired(false);

            builder.Property(da => da.PostalCode) // NOTE: Index, PostalCode
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(da => da.Details)
                .HasMaxLength(255)
                .IsRequired(false);

            // Relation to Customer configured in CustomerConfiguration (WithOne)

            // Relation to Order (1-to-Many) configured in OrderConfiguration

            // Seed data
            builder.HasData(
                new DeliveryAddress 
                {
                    Id = 1,
                    AddressAlias = "Home",
                    Country = "Germany",
                    City = "Berlin",
                    AddressLine1 = "Unter den Linden 77",
                    PostalCode = "10117",
                    CustomerId = 1,
                }
            );
        }
    }
}
