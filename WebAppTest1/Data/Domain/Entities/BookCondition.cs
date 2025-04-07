namespace AntiqueBookstore.Domain.Entities
{
    public class BookCondition
    {
        // Entity properties
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Initialize to empty string; Refers to 'Excellent, VeryGood, Good, Fair, Poor, Damaged'
        public string? Description { get; set; } // Nullable
        public bool IsActive { get; set; }

        // Navigation property
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
