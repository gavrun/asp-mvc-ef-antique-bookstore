using System.ComponentModel.DataAnnotations.Schema;

namespace AntiqueBookstore.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? PaymentDate { get; set; }


        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!; // Required

        public int EmployeeId { get; set; } // Employee who placed the Order
        public virtual Employee Employee { get; set; } = null!; // Required

        public int? DeliveryAddressId { get; set; } // Nullable, for example, self-pickup
        public virtual DeliveryAddress? DeliveryAddress { get; set; }

        public int OrderStatusId { get; set; }
        public virtual OrderStatus OrderStatus { get; set; } = null!; // Required

        public int PaymentMethodId { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; } = null!; // Required

        // Navigation property to Sale 
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();


        // TODO: Add a calculated property for the Order total
        [NotMapped]
        public decimal TotalAmount => Sales.Sum(s => s.SalePrice);
    }
}
