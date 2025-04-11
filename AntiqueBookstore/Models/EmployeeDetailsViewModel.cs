using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class EmployeeDetailsViewModel
    {
        public int Id { get; set; } 


        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }


        [Display(Name = "Active")]
        public bool IsActive { get; set; }


        // Current position
        [Display(Name = "Current Position")]
        public string? CurrentPositionTitle { get; set; }

        [Display(Name = "Current Level")]
        public string? CurrentLevelName { get; set; }


        // Position history
        [Display(Name = "Position History")]
        public List<PositionHistoryViewModel> PositionHistory { get; set; } = new List<PositionHistoryViewModel>();

        // INFO: Potentially link ApplicationUserId 
        // However, not clear how to keep user id and email history

        // public string? ApplicationUserId { get; set; }
        // [Display(Name = "User Email")]
        // public string? UserEmail { get; set; } 
    }
}
