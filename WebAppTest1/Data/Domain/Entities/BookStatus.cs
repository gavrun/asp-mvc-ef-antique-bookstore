namespace AntiqueBookstore.Domain.Entities
{
    public class BookStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Refers to 'InStock, OnOrder, Sold', friendly 'Available, Reserved, Sold, Archived'
        public bool IsActive { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
