namespace AntiqueBookstore.Domain.Entities
{
    public class DeliveryAddress
    {
        public int Id { get; set; }
        public string? AddressAlias { get; set; } // Refers to 'Home', 'Work', etc.
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; } // Nullable
        public string PostalCode { get; set; } = string.Empty; // NOTE: Named Postal Index or ZIP code as PostalCode
        public string? Details { get; set; } // Nullable

        // Foreign key to Customer (1-to-Many)
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!; // Required

        // Navigation property to Order, (1-to-Many) one address can be in several Orders
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
