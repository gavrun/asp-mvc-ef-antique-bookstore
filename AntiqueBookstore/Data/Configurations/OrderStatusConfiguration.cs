using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.HasKey(os => os.Id);

            builder.Property(os => os.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(os => os.Name).IsUnique();

            builder.Property(os => os.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasData(
                new OrderStatus { Id = 1, Name = "New", IsActive = true },
                new OrderStatus { Id = 2, Name = "Processing", IsActive = true },
                new OrderStatus { Id = 3, Name = "Shipped", IsActive = true },
                new OrderStatus { Id = 4, Name = "Delivered", IsActive = true },
                new OrderStatus { Id = 5, Name = "Cancelled", IsActive = true },
                new OrderStatus { Id = 6, Name = "Payment Pending", IsActive = true },
                new OrderStatus { Id = 7, Name = "Paid", IsActive = true }
            );
        }
    }
}
