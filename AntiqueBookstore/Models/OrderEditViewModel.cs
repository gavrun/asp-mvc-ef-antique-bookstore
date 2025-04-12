using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class OrderEditViewModel
    {
        // Order ID to edit (main reason this ViewModel was created)
        public int Id { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime OrderDate { get; set; } // load existing

        [Display(Name = "Customer")]
        [Required(ErrorMessage = "Please select a customer.")]
        public int SelectedCustomerId { get; set; }

        public SelectList? Customers { get; set; }

        [Display(Name = "Sales Representative")]
        [Required(ErrorMessage = "Please select a sales representative.")]
        public int SelectedEmployeeId { get; set; }

        public SelectList? Employees { get; set; }

        [Display(Name = "Order Status")]
        [Required(ErrorMessage = "Please select an order status.")]
        public int SelectedOrderStatusId { get; set; }

        public SelectList? OrderStatuses { get; set; }

        [Display(Name = "Payment Method")]
        [Required(ErrorMessage = "Please select a payment method.")]
        public int SelectedPaymentMethodId { get; set; }

        public SelectList? PaymentMethods { get; set; }

        [Display(Name = "Available Books")]
        public SelectList? AvailableBooks { get; set; } // to be added to Order

        // Existing added Books in the Order (SaleCreateItemViewModel)
        public List<SaleCreateItemViewModel> Sales { get; set; } = new List<SaleCreateItemViewModel>();


        // public int? SelectedDeliveryAddressId { get; set; }
        // public SelectList? DeliveryAddresses { get; set; } // requires dynamic loading

    }
}
