// Models/FeedBack.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticsWebApp.Models // Adjust this namespace
{
    public class FeedBack
    {
        [Key]
        public int FeedbackID { get; set; } // 

        // Foreign Key to Invoice
        [Required]
        public int InvoiceID { get; set; }
        [ForeignKey("InvoiceID")]
        public Invoice Invoice { get; set; } // 

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")] // Star rating validation
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        [DataType(DataType.MultilineText)] // Hint for UI to use a textarea
        public string? Message { get; set; } // Message can be optional or required based on  needs. 

        // You can add more validation attributes or properties as needed
    }
}