using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList.Core;

namespace AntiqueBookstore.Models
{
    public class AuditLogViewModel
    {
        public List<SalesAuditLog>? AuditLogs { get; set; }

        // Paginated list of Audit logs
        //public IPagedList<SalesAuditLog>? AuditLogs { get; set; }


        public SelectList? LoginList { get; set; }

        public string? SelectedLogin { get; set; }


        // Properties for manual pagination 
        // public int CurrentPage { get; set; }
        // public int TotalPages { get; set; }
    }
}
