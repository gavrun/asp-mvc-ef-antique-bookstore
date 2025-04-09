using Microsoft.AspNetCore.Identity;

namespace AntiqueBookstore.Domain.Entities
{
    public class ApplicationUser : IdentityUser // a base user from Identity
    {
        // extend properties of a system user


        // NOTE: Link to Employee
        public int? EmployeeId { get; set; }

        // Navigation property to Employee
        public virtual Employee? Employee { get; set; }

    }
}
