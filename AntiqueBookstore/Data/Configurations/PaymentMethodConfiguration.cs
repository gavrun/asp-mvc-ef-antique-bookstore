using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(pm => pm.Id);

            builder.Property(pm => pm.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(pm => pm.Name).IsUnique();

            builder.Property(pm => pm.IsActive)
                .IsRequired();
                //.HasDefaultValue(true);

            builder.HasData(
                new PaymentMethod { Id = 1, Name = "Credit Card", IsActive = true },
                new PaymentMethod { Id = 2, Name = "Cash", IsActive = true },
                new PaymentMethod { Id = 3, Name = "PayPal", IsActive = false }, // Example of a payment method that is 
                new PaymentMethod { Id = 4, Name = "Bank Transfer", IsActive = false } // 'to be implemented'
            );
        }
    }
}
