// Models/TransportUnit.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // For ForeignKey attribute

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
        public Lorry? Lorry { get; set; } // Navigation property

        // Foreign Key for Driver
        [Required] // A TransportUnit must have a Driver
        public int DriverID { get; set; }
        [ForeignKey("DriverID")]
        public Driver? Driver { get; set; } // Navigation property

        // Foreign Key for Assistant (optional as per some diagrams, but let's make it required for now, can change if needed)
        [Required] // A TransportUnit must have an Assistant
        public int AssistantID { get; set; }
        [ForeignKey("AssistantID")]
        public Assistant? Assistant { get; set; } // Navigation property

        [Required]
        [StringLength(100)] // Assuming container ID/description
        public string? Container { get; set; }

        [Required]
        [StringLength(50)] // e.g., "Available", "In Transit", "Maintenance"
        public string? Status { get; set; }
    }
}