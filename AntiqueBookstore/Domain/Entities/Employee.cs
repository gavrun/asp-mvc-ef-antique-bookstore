namespace AntiqueBookstore.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public string? Comment { get; set; }

        // Relation to ApplicationUser
        public string? ApplicationUserId { get; set; } // Nullable
        // NOTE: Protect, Employee can be created before ApplicationUser

        // Navigation property to ApplicationUser
        public virtual ApplicationUser? ApplicationUser { get; set; }
        //public virtual ApplicationUser User { get; set; }

        // Navigation property to PositionHistory
        public virtual ICollection<PositionHistory> PositionHistories { get; set; } = new List<PositionHistory>();

        // Navigation property to Order
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
