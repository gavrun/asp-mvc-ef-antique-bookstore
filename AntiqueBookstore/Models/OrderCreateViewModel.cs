﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AntiqueBookstore.Models
{
    public class OrderCreateViewModel
    {
        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Today;

        [Display(Name = "Customer")]
        [Required(ErrorMessage = "Please select a customer.")]
        public int SelectedCustomerId { get; set; }

        public SelectList? Customers { get; set; } // dropdown

        [Display(Name = "Sales Representative")]
        [Required(ErrorMessage = "Please select a sales representative.")]
        public int SelectedEmployeeId { get; set; }

        public SelectList? Employees { get; set; } // dropdown

        [Display(Name = "Order Status")]
        [Required(ErrorMessage = "Please select an order status.")]
        public int SelectedOrderStatusId { get; set; }

        public SelectList? OrderStatuses { get; set; } // dropdown

        [Display(Name = "Payment Method")]
        [Required(ErrorMessage = "Please select a payment method.")]
        public int SelectedPaymentMethodId { get; set; }

        public SelectList? PaymentMethods { get; set; } // dropdown

        [Display(Name = "Available Books")]
        public SelectList? AvailableBooks { get; set; } // dropdown to add Books

        // Binding added Books over a List
        // populated by model binding from indexed fields
        // generated by JS (e.g., Sales[0].BookId, Sales[0].SalePrice, ...)
        public List<SaleCreateItemViewModel> Sales { get; set; } = new List<SaleCreateItemViewModel>();


        // TODO: DeliveryAddress was not implemented because auto scaffolding for Customers

        // public int? SelectedDeliveryAddressId { get; set; }
        // public SelectList? DeliveryAddresses { get; set; }

    }
}
