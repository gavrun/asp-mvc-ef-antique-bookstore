using AntiqueBookstore.Domain.Enums;

namespace AntiqueBookstore.Domain.Entities
{
    public class Position
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // enum for position type
        public WorkSchedule WorkSchedule { get; set; }

        // Foreign key 
        public int RoleId { get; set; } // FK to Role

        // Navigation property for the many-to-many relationship with Role
        public virtual Role Role { get; set; } = null!;

        // Navigation property for the many-to-many relationship with PositionHistory
        public virtual ICollection<PositionHistory> PositionHistories { get; set; } = new List<PositionHistory>();
    }
}
