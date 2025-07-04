﻿// Models/Assistant.cs
using System.ComponentModel.DataAnnotations;

namespace LogisticsWebApp.Models
{
    public class Assistant
    {
        [Key] // Specifies AssistantID as the primary key
        public int AssistantID { get; set; }

        [Required] // Name is required
        [StringLength(100)] // Max length for Name
        public string Name { get; set; }

        [Required] // Phone is required
        [StringLength(20)] // Max length for Phone
        [Phone] // Data annotation for phone number format validation
        public string Phone { get; set; }

        [Range(18, 99)] // Age should be between 18 and 99
        public int Age { get; set; }
    }
}