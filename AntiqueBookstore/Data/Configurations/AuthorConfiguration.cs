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
                new Author { Id = 1, FirstName = "Arthur", LastName = "Conan Doyle", BirthYear = 1859, DeathYear = 1930, Bio = "British writer" }
                );
        }
    }
}
