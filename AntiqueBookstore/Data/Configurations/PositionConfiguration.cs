using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(p => p.Title).IsUnique();

            builder.Property(p => p.WorkSchedule)
                .IsRequired();
            // By default writes enum as int 

            // Relationship to Role with in PK
            builder.HasOne(p => p.Role)
                   .WithMany(r => r.Positions)
                   .HasForeignKey(p => p.RoleId)
                   .OnDelete(DeleteBehavior.Restrict); // Cannot delete Role if there are Positions
        }
    }
}
