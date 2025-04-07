using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(a => new { a.LastName, a.FirstName });

            builder.Property(a => a.Bio)
                .IsRequired(false);
            // By default nvarchar(max) or TEXT


            // NOTE: Constraints on 'BirthYear' and 'DeathYear' can be added at the application/validation level

            // Seed data
            builder.HasData(
                new Author { Id = 1, FirstName = "Arthur", LastName = "Conan Doyle", BirthYear = 1859, DeathYear = 1930, Bio = "British writer" },
                new Author { Id = 2, FirstName = "Murasaki", LastName = "Shikibu", BirthYear = 978, DeathYear = 1016, Bio = "Japanese novelist and poet of the Heian period" },
                new Author { Id = 3, FirstName = "Lu", LastName = "Xun", BirthYear = 1881, DeathYear = 1936, Bio = "Chinese writer, considered the founder of modern Chinese literature" },
                new Author { Id = 4, FirstName = "Naguib", LastName = "Mahfouz", BirthYear = 1911, DeathYear = 2006, Bio = "Egyptian writer who won the Nobel Prize for Literature" },
                new Author { Id = 5, FirstName = "Chinua", LastName = "Achebe", BirthYear = 1930, DeathYear = 2013, Bio = "Nigerian novelist, poet and critic" },
                new Author { Id = 6, FirstName = "Jorge Luis", LastName = "Borges", BirthYear = 1899, DeathYear = 1986, Bio = "Argentine short-story writer, essayist and poet" },
                new Author { Id = 7, FirstName = "Mikhail", LastName = "Bulgakov", BirthYear = 1891, DeathYear = 1940, Bio = "Russian writer, physician and playwright" },
                new Author { Id = 8, FirstName = "James", LastName = "Joyce", BirthYear = 1882, DeathYear = 1941, Bio = "Irish novelist, short story writer and poet" },
                new Author { Id = 9, FirstName = "Gabriel", LastName = "García Márquez", BirthYear = 1927, DeathYear = 2014, Bio = "Colombian novelist and Nobel Prize winner" },
                new Author { Id = 10, FirstName = "Umberto", LastName = "Eco", BirthYear = 1932, DeathYear = 2016, Bio = "Italian novelist, literary critic and philosopher" },
                new Author { Id = 11, FirstName = "Rabindranath", LastName = "Tagore", BirthYear = 1861, DeathYear = 1941, Bio = "Bengali polymath and Nobel Prize winner" }
                );
        }
    }
}
