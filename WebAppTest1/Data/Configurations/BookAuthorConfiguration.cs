using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            // Composite key
            builder.HasKey(ba => new { ba.BookId, ba.AuthorId });

            // Relation to Books
            builder.HasOne(ba => ba.Book)
                   .WithMany(b => b.BookAuthors) // Navigation property in Books
                   .HasForeignKey(ba => ba.BookId)
                   .OnDelete(DeleteBehavior.Cascade); // When deleting Books, delete the associated BookAuthor mapping

            // Relation to Author
            builder.HasOne(ba => ba.Author)
                   .WithMany(a => a.BookAuthors) // Using the navigation property in Author
                   .HasForeignKey(ba => ba.AuthorId)
                   .OnDelete(DeleteBehavior.Restrict); // Cannot delete Author if it is associated with Books.
                                                       // NOTE: Cascade is dangerous here, deleting the Author might delete the Books

            // Seed data
            builder.HasData(
                new BookAuthor { BookId = 1, AuthorId = 1 } // Book 1 with Author 1
                );
        }
    }
}
