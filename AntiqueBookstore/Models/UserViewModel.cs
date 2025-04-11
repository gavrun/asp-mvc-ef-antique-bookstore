namespace AntiqueBookstore.Models
{
    public class UserViewModel
    {
        // UserViewModel + UserManagementViewModel class corresponding to User entity for UserManagement

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public IList<string> Roles { get; set; } // currently available Identity Roles
        public string LinkedEmployeeName { get; set; } // Full name of the linked employee FirstName + LastName
        public int? LinkedEmployeeId { get; set; } // Id of the linked employee, Nullable, protective programming

        public bool IsLinked => LinkedEmployeeId.HasValue; // if the User is linked to Employee
    }
}
