namespace AntiqueBookstore.Domain.Entities
{
    public class SalesAuditLog
    {
        // NOTE: Review approach: string (per-column) vs. Collection/JSON (snapshot)

        // Column based logging 
        public long EventId { get; set; } // NOTE: Named PK
        // TODO: EventId data type should be long (int64) for large tables instead of int (int32)
        public DateTime Timestamp { get; set; } // Event timestamp derived from Interceptor
        public string TableName { get; set; } = string.Empty; // Table name (Entity)
        public string RecordId { get; set; } = string.Empty; // PK of changed record to TableName
        public string Operation { get; set; } = string.Empty; // Operation type (INSERT, UPDATE, DELETE)
        public string? ColumnName { get; set; } // Column name (UPDATE), Nullable (INSERT, DELETE)
        public string? OldValue { get; set; } // Old value (UPDATE, DELETE), Nullable (INSERT)
        public string? NewValue { get; set; } // New value (INSERT, UPDATE), Nullable (DELETE)
        public string? Login { get; set; } // User login name derived from HttpContext
    }
}
