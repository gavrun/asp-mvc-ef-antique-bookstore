using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderDate)
                .IsRequired(true); // NOTE: OrderDate is required when creating an Order

            builder.Property(o => o.DeliveryDate)
                .IsRequired(false);

            builder.Property(o => o.PaymentDate)
                .IsRequired(false);

            // Relation to Customer
            builder.HasOne(o => o.Customer)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(o => o.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict); // Cannot delete a Customer if linked to Orders

            // Relation to Employee
            builder.HasOne(o => o.Employee)
                   .WithMany(e => e.Orders)
                   .HasForeignKey(o => o.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict); // Cannot delete an Employee if linked to Orders

            // Relation to DeliveryAddress (Nullable, FK)
            builder.HasOne(o => o.DeliveryAddress)
                   .WithMany(da => da.Orders)
                   .HasForeignKey(o => o.DeliveryAddressId)
                   .IsRequired(false) // Адрес не обязателен
                   .OnDelete(DeleteBehavior.Restrict); // Cannot delete the Address if it is used in the Orders
                                                       // NOTE: SetNull, if we want to reset FK in the Order when deleting the Address

            // Relation to OrderStatus
            builder.HasOne(o => o.OrderStatus)
                   .WithMany(os => os.Orders)
                   .HasForeignKey(o => o.OrderStatusId)
                   .OnDelete(DeleteBehavior.Restrict); // Cannot delete a Status if it is in use

            // Relation to PaymentMethod
            builder.HasOne(o => o.PaymentMethod)
                   .WithMany(pm => pm.Orders)
                   .HasForeignKey(o => o.PaymentMethodId)
                   .OnDelete(DeleteBehavior.Restrict); // Cannot delete a payment method if it is in use

            // Relation to Sale (1-to-Many)
            builder.HasMany(o => o.Sales)
                   .WithOne(s => s.Order)
                   .HasForeignKey(s => s.OrderId)
                   .OnDelete(DeleteBehavior.Cascade); // Delete Sales items when deleting an Order
        }
    }
}
