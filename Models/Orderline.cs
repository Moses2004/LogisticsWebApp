// Models/Orderline.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // Required for IdentityUser

namespace LogisticsWebApp.Models
{
    public class Orderline
    {
        [Key]
        public int OrderlineID { get; set; }

        // Foreign key to AspNetUsers table (IdentityUser)
        [Required]
        public required string CustomerID { get; set; } // Added 'required' keyword here

       

        [ForeignKey("CustomerID")]
        // Navigation property to the IdentityUser
        // This links an Orderline to a specific customer
        public required IdentityUser Customer { get; set; }

        [Required]
        [StringLength(255)] // Added StringLength for database column size
        public required string InitialAddress { get; set; }

        [Required]
        [StringLength(255)] // Added StringLength for database column size
        public required string DestinationAddress { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [Range(0.1, 10000.0, ErrorMessage = "Weight must be between 0.1 and 10000 Kg.")]
        public double WeightKg { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";
    }
}
