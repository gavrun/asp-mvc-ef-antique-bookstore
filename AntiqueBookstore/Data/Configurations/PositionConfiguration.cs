using AntiqueBookstore.Domain.Entities;
using AntiqueBookstore.Domain.Enums;
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

            // Relationship to Level with in PK
            builder.HasOne(p => p.Level)
                   .WithMany(r => r.Positions)
                   .HasForeignKey(p => p.LevelId)
                   .OnDelete(DeleteBehavior.Restrict); // Cannot delete Level if there are Positions

            // Seed Positions "Store Manager", "Sales Associate"
            builder.HasData(
                new Position { Id = 1, Title = "Store Manager", WorkSchedule = WorkSchedule.FullTime, LevelId = 1 },
                new Position { Id = 2, Title = "Sales Associate", WorkSchedule = WorkSchedule.FullTime, LevelId = 2 }
                );
        }
    }
}
