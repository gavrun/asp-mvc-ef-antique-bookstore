using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class OrderIndexViewModel
    {
        [Display(Name = "Order ID")]
        public int Id { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.Date)] 
        public DateTime OrderDate { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; } // FirstName + LastName

        [Display(Name = "Sales Rep")]
        public string EmployeeName { get; set; } // FirstName + LastName

        [Display(Name = "Status")]
        public string StatusName { get; set; }


        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")] // Format as currency
        public decimal TotalAmount { get; set; }
    }
}
