namespace AntiqueBookstore.Domain.Entities
{
    public class PositionHistory
    {
        public int PromotionId { get; set; } // PK
        public DateTime StartDate { get; set; }

        // Current Position
        public DateTime? EndDate { get; set; } // Nullable 

        // NOTE: Protect, PositionHistory can be created by mistake
        public bool IsActive { get; set; }

        // Navigation property to Employee
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!; // Required

        // Navigation property to Position
        public int PositionId { get; set; }
        public virtual Position Position { get; set; } = null!; // Required
    }
}
