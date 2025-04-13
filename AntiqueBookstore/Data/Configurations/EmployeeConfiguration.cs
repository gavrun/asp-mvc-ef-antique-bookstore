using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntiqueBookstore.Data.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.HireDate)
                .IsRequired();

            builder.Property(e => e.IsActive)
                .IsRequired();
                //.HasDefaultValue(true);

            builder.Property(e => e.Comment)
                .HasMaxLength(500)
                .IsRequired(false);

            // Identity
            // Relation to ApplicationUser, 1-to-1, configured here, or in ApplicationUserConfiguration (not implemented)
            builder.HasOne(e => e.ApplicationUser)
                   .WithOne(au => au.Employee) // NOTE: Navigation property in ApplicationUser
                   .HasForeignKey<Employee>(e => e.ApplicationUserId) // FK to Employee
                   .IsRequired(false) // Employee exists without an Identity user reference
                   .OnDelete(DeleteBehavior.SetNull); // Reset FK for the Employee if IdentityUser is deleted

            // NOTE: Protect, make sure ApplicationUserId can store Guid (Identity)
            builder.Property(e => e.ApplicationUserId)
                  .HasMaxLength(450) // Default length for Identity keys
                  .IsRequired(false);

            builder.HasIndex(e => e.ApplicationUserId).IsUnique(); // One User - one Employee


            // Relation to Order (1-to-Many) configured in OrderConfiguration

            // Relation to PositionHistory (1-to-Many) 
            builder.HasMany(e => e.PositionHistories)
                   .WithOne(ph => ph.Employee) // Navigation property in Employee
                   .HasForeignKey(ph => ph.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade); // When deleting Employee, we delete his job history

            // Seed data
            builder.HasData(
                new Employee
                {
                    Id = 1,
                    FirstName = "Jane",
                    LastName = "Smith",
                    HireDate = new DateTime(2023, 1, 15),
                    IsActive = true,
                    ApplicationUserId = null 
                },
                new Employee
                {
                    Id = 2,
                    FirstName = "Bob",
                    LastName = "Williams",
                    HireDate = new DateTime(2023, 3, 10),
                    IsActive = true,
                    ApplicationUserId = null 
                },
                new Employee
                {
                    Id = 3,
                    FirstName = "New",
                    LastName = "Unlinked",
                    HireDate = new DateTime(2023, 5, 1),
                    IsActive = false,
                    ApplicationUserId = null // will stay null after seeding 
                }
            );
        }
    }
}
