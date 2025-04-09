namespace AntiqueBookstore.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; } // NOTE: Add Email of course, Nullable
        public string? Phone { get; set; } // Nullable
        public bool IsActive { get; set; }
        public string? Comment { get; set; }

        // Navigation property to DeliveryAddress
        public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();

        // Navigation property to Order
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
