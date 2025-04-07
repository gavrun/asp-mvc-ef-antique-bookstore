using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations 
{
    public class SalesAuditLogConfiguration : IEntityTypeConfiguration<SalesAuditLog>
    {
        // BUG: Review string vs. Collection/JSON 
        public void Configure(EntityTypeBuilder<SalesAuditLog> builder)
        {
            builder.HasKey(log => log.EventId); // NOTE: Named PK

            builder.Property(log => log.EventId)
                .ValueGeneratedOnAdd(); // Auto-increment for EventId

            builder.Property(log => log.TableName)
                .IsRequired()
                .HasMaxLength(128); // Table name length avg

            builder.Property(log => log.RecordId)
                .IsRequired()
                .HasMaxLength(100); // PK length avg, based on enire DB

            builder.Property(log => log.Operation)
                .IsRequired()
                .HasMaxLength(10); // Operation type length, INSERT, UPDATE, DELETE

            builder.Property(log => log.ColumnName)
                .HasMaxLength(128) // Column name length avg
                .IsRequired(false); // NULL for INSERT, DELETE

            builder.Property(log => log.OldValue)
                   .IsRequired(false);

            builder.Property(log => log.NewValue)
                   .IsRequired(false);

            builder.Property(log => log.Login)
                .HasMaxLength(256) // Стандартная длина для Identity User Name/Email
                .IsRequired(false);


            // INFO: Performance, Indices not required for Audit Log which is not used for queries

            //builder.HasIndex(log => log.Timestamp);

            //builder.HasIndex(log => new { log.TableName, log.RecordId, log.Timestamp })
            //       .HasDatabaseName("IX_SalesAuditLog_RecordHistory");

            //builder.HasIndex(log => new { log.TableName, log.RecordId, log.ColumnName, log.Timestamp })
            //       .HasDatabaseName("IX_SalesAuditLog_ColumnHistory");
        }
    }
}
