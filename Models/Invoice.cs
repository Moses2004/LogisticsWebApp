// Models/Invoice.cs
using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticsWebApp.Models // Adjust this namespace to match your project's Models namespace
{
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }

        // Foreign Key to Orderline
        [Required] // Assuming an invoice must be linked to an orderline
        public int OrderlineID { get; set; }
        [ForeignKey("OrderlineID")]
        public Orderline? Orderline { get; set; } // Navigation property

        // Foreign Key to TransportUnit
        [Required] // Assuming an invoice must be linked to a transport unit
        public int TransportUnitID { get; set; }
        [ForeignKey("TransportUnitID")]
        public TransportUnit? TransportUnit { get; set; } // Navigation property

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Example for decimal precision
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)] // Example max length for status string
        public string PaymentStatus { get; set; } // e.g., "Pending", "Paid", "Cancelled"

        public string? CustomerId { get; set; } // Foreign key to IdentityUser.Id. 
        [ForeignKey("CustomerId")]
        public IdentityUser? Customer { get; set; }


        public DateTime? InvoiceDate { get; set; }
    }
}