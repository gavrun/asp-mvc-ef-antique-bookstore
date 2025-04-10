using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class BookEditViewModel
    {
        // BookEditViewModel class corresponding to the Book entity

        public int Id { get; set; }

        public string? ExistingCoverImagePath { get; set; }

        // Book properties

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(255, ErrorMessage = "Title cannot be longer than 255 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Publisher name cannot be longer than 100 characters.")]
        public string? Publisher { get; set; }

        [Required(ErrorMessage = "Publication year is required.")]
        [Range(1000, 2099, ErrorMessage = "Please enter a valid year (e.g., 1000-2099).")] 
        [Display(Name = "Publication Year")]
        public int PublicationDate { get; set; }

        [Range(0.00, 9999999.99, ErrorMessage = "Purchase price must be a non-negative value.")] // max and precision(9,2)
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)] // format
        [Display(Name = "Purchase Price")]
        public decimal? PurchasePrice { get; set; } 

        [Range(0.00, 9999999.99, ErrorMessage = "Recommended price must be a non-negative value.")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Recommended Price")]
        public decimal? RecommendedPrice { get; set; }


        // Relations

        [Required(ErrorMessage = "Please select a condition.")]
        [Display(Name = "Condition")]
        public int ConditionId { get; set; }

        [Required(ErrorMessage = "Please select a status.")]
        [Display(Name = "Status")]
        public int StatusId { get; set; }

        [Required(ErrorMessage = "Please select at least one author.")]
        [Display(Name = "Authors")]
        public List<int> SelectedAuthorIds { get; set; } = new List<int>();


        // Fill <select> elements
        public IEnumerable<SelectListItem>? ConditionsList { get; set; }
        public IEnumerable<SelectListItem>? StatusesList { get; set; }
        public IEnumerable<SelectListItem>? AuthorsList { get; set; }

    }
}
