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
                .IsRequired()
                .HasDefaultValue(true);

            // Relation to Employee configured in EmployeeConfiguration (WithOne)

            // Relation to Position
            builder.HasOne(ph => ph.Position)
                   .WithMany(p => p.PositionHistories) // Using the navigation property in Position
                   .HasForeignKey(ph => ph.PositionId)
                   .OnDelete(DeleteBehavior.Restrict); // Cannot delete Position if exists in history
        }
    }
}
