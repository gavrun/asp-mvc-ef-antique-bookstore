namespace AntiqueBookstore.Domain.Entities
{
    public class Author
    {
        // Entity properties
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? BirthYear { get; set; } // Nullable
        public int? DeathYear { get; set; } // Nullable
        public string? Bio { get; set; } // Nullable

        // Navigation property for the many-to-many relationship with Books
        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
