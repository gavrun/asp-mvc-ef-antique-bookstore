using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class PositionHistoryConfiguration : IEntityTypeConfiguration<PositionHistory>
    {
        public void Configure(EntityTypeBuilder<PositionHistory> builder)
        {
            builder.HasKey(ph => ph.PromotionId);

            builder.Property(ph => ph.PromotionId).ValueGeneratedOnAdd();

            builder.Property(ph => ph.StartDate)
                .IsRequired();

            builder.Property(ph => ph.EndDate)
                .IsRequired(false); // Nullable

            builder.Property(ph => ph.IsActive)
                .IsRequired();
                //.HasDefaultValue(true);

            // Relation to Employee configured in EmployeeConfiguration (WithOne)

            // Relation to Position
            builder.HasOne(ph => ph.Position)
                   .WithMany(p => p.PositionHistories) // Using the navigation property in Position
                   .HasForeignKey(ph => ph.PositionId)
                   .OnDelete(DeleteBehavior.Restrict); // Cannot delete Position if exists in history

            // Seed data 
            builder.HasData(
                new PositionHistory
                {
                    PromotionId = 1,
                    StartDate = new DateTime(2023, 1, 15),
                    EndDate = null,
                    IsActive = true,
                    EmployeeId = 1,
                    PositionId = 1
                },
                new PositionHistory
                {
                    PromotionId = 2,
                    StartDate = new DateTime(2023, 3, 10),
                    EndDate = null,
                    IsActive = true,
                    EmployeeId = 2,
                    PositionId = 2
                }
                );
        }
    }
}
