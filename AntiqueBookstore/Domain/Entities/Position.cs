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
        public int LevelId { get; set; } // FK to Level

        // Navigation property for the many-to-many relationship with Level
        public virtual Level Level { get; set; } = null!;

        // Navigation property for the many-to-many relationship with PositionHistory
        public virtual ICollection<PositionHistory> PositionHistories { get; set; } = new List<PositionHistory>();
    }
}
