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
                new DeliveryAddress { Id = 1, AddressAlias = "Home", Country = "Germany", City = "Berlin", AddressLine1 = "Unter den Linden 77", PostalCode = "10117", CustomerId = 1 },
                new DeliveryAddress { Id = 2, AddressAlias = "Home", Country = "France", City = "Paris", AddressLine1 = "15 Avenue des Champs-Élysées", PostalCode = "75008", CustomerId = 2 },
                new DeliveryAddress { Id = 3, AddressAlias = "Home", Country = "Japan", City = "Tokyo", AddressLine1 = "4-2-8 Shinjuku, Shinjuku-ku", PostalCode = "160-0022", CustomerId = 3 },
                new DeliveryAddress { Id = 4, AddressAlias = "Home", Country = "South Africa", City = "Cape Town", AddressLine1 = "123 Nelson Mandela Boulevard", PostalCode = "8001", CustomerId = 4 },
                new DeliveryAddress { Id = 5, AddressAlias = "Home", Country = "Argentina", City = "Buenos Aires", AddressLine1 = "789 Avenida 9 de Julio", PostalCode = "C1043", CustomerId = 5 }
            );
        }
    }
}
