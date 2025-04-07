using AntiqueBookstore.Domain.Entities;

namespace AntiqueBookstore.Data.Seed
{
    public static class BookCatalogData
    {
        // 
        // Sample data for Book catalog
        // 

        // Authors
        public static IEnumerable<Author> GetAuthors()
        {
            // TODO: Get test authors data

            return Enumerable.Empty<Author>();
        }

        // Books
        public static IEnumerable<Book> GetBooks(ApplicationDbContext context)
        {
            // TODO: Get test books data

            return Enumerable.Empty<Book>();
        }


        // Relations BookAuthor to be seeded after Books and Authors generated IDs
    }
}
