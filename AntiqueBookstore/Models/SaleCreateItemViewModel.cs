using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class SaleCreateItemViewModel
    {
        [Required] 
        public int BookId { get; set; }

        [Required] 
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale price must be positive.")] 
        [DataType(DataType.Currency)] 
        public decimal SalePrice { get; set; } // Currencty format and validation

        // Add Discount (Sales Event) when creating
        public int? EventId { get; set; }
    }
}
