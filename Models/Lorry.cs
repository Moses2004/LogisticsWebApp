// Models/Lorry.cs
using System.ComponentModel.DataAnnotations;

namespace LogisticsWebApp.Models
{
    public class Lorry
    {
        [Key] // Denotes LorryID as the primary key
        public int LorryID { get; set; }

        [Required] // LicensePlate is mandatory
        [StringLength(20, ErrorMessage = "License Plate cannot exceed 20 characters.")] // Set a max length for the string
        public string? LicensePlate { get; set; }

        [Required] // Model is mandatory
        [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters.")] // Set a max length
        public string? Model { get; set; }

        [Required] // Capacity is mandatory
        [Range(1, 100000, ErrorMessage = "Capacity must be between 1 and 100000 Kg.")] // Assuming capacity in Kg
        public decimal CapacityKg { get; set; } // Using decimal for precise capacity

        [Required] // Status is mandatory
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")] // Set a max length
        public string Status { get; set; } = "Available"; // Default status
    }
}
