using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class AuthorCreateAjaxViewModel
    {
        // properties for the AuthorCreateAjaxViewModel class corresponding to the Author entity

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // any annotations ??
        public int? BirthYear { get; set; }

        public int? DeathYear { get; set; }

        public string? Bio { get; set; }


        // TODO: come back for BookAuthors many-to-many mapping
    }
}
