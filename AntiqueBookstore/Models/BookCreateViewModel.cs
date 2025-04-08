using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class BookCreateViewModel
    {
        // properties for the BookCreateViewModel class corresponding to the Book entity

        [Required(ErrorMessage = "Please enter the book title.")]
        [MaxLength(255)] 
        [Display(Name = "Book Title")]
        public string Title { get; set; } = string.Empty;


        [MaxLength(100)] 
        public string? Publisher { get; set; }


        [Required(ErrorMessage = "Please enter the publication year.")]
        [Range(1000, 2100, ErrorMessage = "Please enter a valid year (e.g., 1887).")] // Year range validation 
        [Display(Name = "Publication Year")]
        public int PublicationDate { get; set; }


        // decimal? as Book model has nullable
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [Display(Name = "Purchase Price")]
        [Range(0.01, 9999999.99, ErrorMessage = "Invalid price.")] // decimal(9,2)
        public decimal? PurchasePrice { get; set; }


        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [Display(Name = "Recommended Price")]
        [Range(0.01, 9999999.99, ErrorMessage = "Invalid price.")]
        public decimal? RecommendedPrice { get; set; }


        // Relationships for BookCondition and BookStatus

        [Required(ErrorMessage = "Please select the book condition.")]
        [Display(Name = "Condition")]
        public int ConditionId { get; set; } // FK for BookCondition


        [Required(ErrorMessage = "Please select the book status.")]
        [Display(Name = "Status")]
        public int StatusId { get; set; } // FK for BookStatus


        // Many-to-Many relationship with Authors
        [Required(ErrorMessage = "Please select at least one author.")]
        [Display(Name = "Author(s)")]
        public List<int> SelectedAuthorIds { get; set; } = new List<int>(); // Get IDs from the selected authors


        // Load file
        [Display(Name = "Cover Image")]
        public IFormFile? CoverImageFile { get; set; }


        // Populating Views with data (controller)

        public SelectList? Conditions { get; set; }

        public SelectList? Statuses { get; set; }

        // For the author selection list (can be MultiSelectList or SelectList)
        public MultiSelectList? Authors { get; set; } 
    }
}
