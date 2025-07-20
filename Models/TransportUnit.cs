// Models/TransportUnit.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // For ForeignKey attribute
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; 

namespace LogisticsWebApp.Models
{
    public class TransportUnit
    {
        [Key]
        public int TransportUnitID { get; set; }

        // Foreign Key for Lorry
        [Required] // A TransportUnit must have a Lorry
        public int? LorryID { get; set; }
        [ForeignKey("LorryID")]
        [ValidateNever] // Add this attribute to ignore validation for the navigation property
        public Lorry Lorry { get; set; } // Navigation property

        // Foreign Key for Driver
        [Required] // A TransportUnit must have a Driver
        public int DriverID { get; set; }
        [ForeignKey("DriverID")]
        [ValidateNever] 
        public Driver? Driver { get; set; } // Navigation property

        [Required] // A TransportUnit must have an Assistant
        public int AssistantID { get; set; }
        [ForeignKey("AssistantID")]
        [ValidateNever] // Add this attribute
        public Assistant Assistant { get; set; } // Navigation property

        [Required]
        [StringLength(100)] // Assuming container ID/description
        public string? Container { get; set; }

        [Required]
        [StringLength(50)] // e.g., "Available", "In Transit", "Maintenance"
        public string? Status { get; set; }
    }
}