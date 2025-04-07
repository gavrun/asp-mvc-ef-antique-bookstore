namespace AntiqueBookstore.Domain.Entities
{
    public class Level // instead of IdentityRole
    {
        // Entity properties
        public int Id { get; set; } // in PK
        public string Name { get; set; } = string.Empty; // Reference to a role, store 'Manager', 'Sales' assistant, etc.
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        // Navigation property to Position
        public virtual ICollection<Position> Positions { get; set; } = new List<Position>();
    }
}
