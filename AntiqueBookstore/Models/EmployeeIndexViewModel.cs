using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class EmployeeIndexViewModel
    {
        public int Id { get; set; }


        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;


        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;


        [Display(Name = "Current Position")]
        public string? CurrentPositionTitle { get; set; } // protect, must not be null in business model

        [Display(Name = "Level")] 
        public string? CurrentLevelName { get; set; } // protect

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
