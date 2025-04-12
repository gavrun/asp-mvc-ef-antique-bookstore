using AntiqueBookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AntiqueBookstore.Data.Interceptors
{
    public class SalesAuditInterceptor : ISaveChangesInterceptor
    {
        // Interceptor automatically logs data changes to the SalesAuditLog table

        // DI, get user information from the current HTTP context
        private readonly IHttpContextAccessor _httpContextAccessor;


        public SalesAuditInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }


        // Synchronous interception before saving changes
        public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            // Delegate the auditing logic to a common method
            AuditChanges(eventData.Context);
            return result; // Return the original result
        }

        // Asynchronous interception before saving changes
        public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            // Delegate the auditing logic to a common method
            AuditChanges(eventData.Context);
            // Return the original result wrapped in a ValueTask
            return ValueTask.FromResult(result);
        }

        // Inspecting the DbContext's ChangeTracker
        private void AuditChanges(DbContext? context)
        {
            // Ensure the context is not null
            if (context == null) return;

            // Get the current User's login name, defaults to "System" if HttpContext is unavailable (background tasks, seeding)
            var userLogin = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            // Use UTC time for the timestamp to ensure consistency across time zones
            var timestamp = DateTime.UtcNow;

            // Create a temporary list to hold all audit log entries generated during this SaveChanges call
            var auditEntries = new List<SalesAuditLog>();

            // Retrieve all tracked entries that are in Added, Modified, or Deleted states
            var changedEntries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList(); // ToList() to materialize the collection, preventing modification issues while iterating

            foreach (var entry in changedEntries)
            {
                // IMPORTANT: Skip auditing the SalesAuditLog entity itself to prevent infinite recursion
                if (entry.Entity is SalesAuditLog)
                    continue;

                // Get the table name associated with the entity, or fallback to CLR type name if table name is not mapped
                var tableName = entry.Metadata.GetTableName() ?? entry.Metadata.ClrType.Name;
                // Get the operation type ("Added", "Modified", "Deleted") directly from the entity state
                var operation = entry.State.ToString();
                // Get the primary key value(s) as a string using a helper method
                var recordId = GetPrimaryKeyValues(entry);

                // Handle Modified entities: Log each changed column individually
                if (entry.State == EntityState.Modified)
                {
                    foreach (var property in entry.Properties.Where(p => p.IsModified))
                    {
                        // Get the actual database column name, fallback to property name if not found
                        var columnName = property.Metadata.GetColumnName(StoreObjectIdentifier.Table(tableName, entry.Metadata.GetSchema())) ?? property.Metadata.Name;
                        // Get the original value before the change
                        var oldValue = property.OriginalValue?.ToString();
                        // Get the current (new) value after the change
                        var newValue = property.CurrentValue?.ToString();

                        // Only create a log entry if the value has actually changed
                        // This avoids logging modifications where the value remains the same (setting a property to its existing value)
                        if (oldValue != newValue)
                        {
                            auditEntries.Add(new SalesAuditLog
                            {
                                TableName = tableName,
                                RecordId = recordId,
                                Operation = operation,
                                ColumnName = columnName, // Specific column that changed
                                OldValue = oldValue,
                                NewValue = newValue,
                                Login = userLogin,
                                Timestamp = timestamp
                            });
                        }
                    }
                }
                // Handle Added entities: Log the initial value of each column
                else if (entry.State == EntityState.Added)
                {
                    foreach (var property in entry.Properties)
                    {
                        // Get the actual database column name, fallback to property name
                        var columnName = property.Metadata.GetColumnName(StoreObjectIdentifier.Table(tableName, entry.Metadata.GetSchema())) ?? property.Metadata.Name;
                        // Get the initial value being inserted
                        var newValue = property.CurrentValue?.ToString();

                        auditEntries.Add(new SalesAuditLog
                        {
                            TableName = tableName,
                            RecordId = recordId, // Note: PK might be temporary if DB-generated (e.g., IDENTITY), EF handles mapping later.
                            Operation = operation,
                            ColumnName = columnName, // Log each column individually
                            OldValue = null, // No old value for an Added record
                            NewValue = newValue,
                            Login = userLogin,
                            Timestamp = timestamp
                        });
                    }
                }
                // Handle Deleted entities: Log the value of each column before deletion
                else if (entry.State == EntityState.Deleted)
                {
                    foreach (var property in entry.Properties)
                    {
                        // Get the actual database column name, fallback to property name
                        var columnName = property.Metadata.GetColumnName(StoreObjectIdentifier.Table(tableName, entry.Metadata.GetSchema())) ?? property.Metadata.Name;
                        // IMPORTANT: Get the value from OriginalValues for Deleted entities
                        var oldValue = entry.OriginalValues[property.Metadata]?.ToString();

                        auditEntries.Add(new SalesAuditLog
                        {
                            TableName = tableName,
                            RecordId = recordId,
                            Operation = operation,
                            ColumnName = columnName, // Log each column individually
                            OldValue = oldValue,
                            NewValue = null, // No new value for a Deleted record
                            Login = userLogin,
                            Timestamp = timestamp
                        });
                    }
                }
            }

            // If any audit log entries were generated, add them to the DbContext
            // They will be saved as part of the same transaction as the original changes
            if (auditEntries.Any())
            {
                context.Set<SalesAuditLog>().AddRange(auditEntries);
            }
        }

        // Helper method to get primary key value(s) from an EntityEntry
        private string GetPrimaryKeyValues(EntityEntry entry)
        {
            // Find the primary key definition for the entity type
            var primaryKey = entry.Metadata.FindPrimaryKey();
            if (primaryKey == null) return "[UNKNOWN PK]"; // Should not happen for most entities

            // Select the current value of each property that is part of the primary key
            // Join the values with a comma if it's a composite key
            return string.Join(",", primaryKey.Properties
                .Select(p => entry.Property(p.Name).CurrentValue?.ToString() ?? "[NULL]"));
        }

    }
}
