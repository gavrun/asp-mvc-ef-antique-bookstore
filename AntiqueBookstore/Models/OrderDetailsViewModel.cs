using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class OrderDetailsViewModel
    {
        [Display(Name = "Order ID")]
        public int Id { get; set; } 

        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Delivery Date")]
        [DataType(DataType.Date)]
        public DateTime? DeliveryDate { get; set; } // nullable dates

        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        public DateTime? PaymentDate { get; set; } // nullable dates

        [Display(Name = "Customer")]
        public string CustomerName { get; set; } = string.Empty; // FirstName + LastName

        public int CustomerId { get; set; } // ID for potential links

        [Display(Name = "Sales Rep")]
        public string EmployeeName { get; set; } = string.Empty;

        public int EmployeeId { get; set; } // ID for potential links

        [Display(Name = "Status")]
        public string StatusName { get; set; } = string.Empty;

        public int OrderStatusId { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethodName { get; set; } = string.Empty;

        public int PaymentMethodId { get; set; }

        [Display(Name = "Delivery Address")]
        public string? DeliveryAddressString { get; set; } // Formatted address or "N/A"

        public int? DeliveryAddressId { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Items in Order")]
        public ICollection<SaleViewModel> Sales { get; set; } = new List<SaleViewModel>(); // List of items
    }
}
