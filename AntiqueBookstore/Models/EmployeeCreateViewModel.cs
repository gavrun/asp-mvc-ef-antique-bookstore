using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class EmployeeCreateViewModel
    {
        // properties for the EmployeeCreateViewModel class corresponding to the Employee entity and its dependencies

        // Id

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hire date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; } = DateTime.Today; // defatuls to today

        public string? Comment { get; set; }

        // ApplicationUserId

        [Required(ErrorMessage = "Position must be selected.")]
        [Display(Name = "Position")]
        public int SelectedPositionId { get; set; } // assigned position ID

        // List of available positions
        public SelectList? Positions { get; set; }
    }
}
