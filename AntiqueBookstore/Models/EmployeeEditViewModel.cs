using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class EmployeeEditViewModel
    {
        // ID mandatory to be present
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hire date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [Required(ErrorMessage = "Position must be selected.")]
        [Display(Name = "Position")]
        public int SelectedPositionId { get; set; } // ID current position

        // List of available positions
        public SelectList? Positions { get; set; }

        // public string? CurrentPositionTitle { get; set; }
        // public DateTime? CurrentPositionStartDate { get; set; }


        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }


        // NOTE: Link to ApplicationUser, BLOCKED: implemented in a separate page to avoid confusion

        //[Display(Name = "Linked User Account")]
        //public string? SelectedApplicationUserId { get; set; } // ID of the selected user (can be null/empty)

        //[Display(Name = "Current User Email")]
        //public string? CurrentUserEmail { get; set; }

        //public SelectList? ApplicationUsers { get; set; } // List of available users
    }
}
