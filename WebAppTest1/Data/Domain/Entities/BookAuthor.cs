namespace AntiqueBookstore.Domain.Entities
{
    public class BookAuthor
    {
        // Many-to-many Books Author by Composite key in Fluent API

        public int BookId { get; set; }
        public virtual Book Book { get; set; } = null!; // Required

        public int AuthorId { get; set; }
        public virtual Author Author { get; set; } = null!; // Required
    }
}
