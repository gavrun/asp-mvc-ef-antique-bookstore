using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class SaleViewModel
    {
        // Sale Id
        // public int Id { get; set; }

        [Display(Name = "Book ID")]
        public int BookId { get; set; }

        [Display(Name = "Book Title")]
        public string BookTitle { get; set; } = string.Empty; // Book title not null

        [Display(Name = "Price Sold")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal SalePrice { get; set; }

        // applied discounts EventId or EventName 
        [Display(Name = "Discount")]
        public string? EventName { get; set; }
    }
}
