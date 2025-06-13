// Models/FeedBack.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticsWebApp.Models // Adjust this namespace
{
    public class FeedBack
    {
        [Key]
        public int FeedbanckID { get; set; } // Note: Name is 'FeedbanckID' as per ERD

        // Foreign Key to Invoice
        [Required]
        public int InvoiceID { get; set; }
        [ForeignKey("InvoiceID")]
        public Invoice? Invoice { get; set; } // Navigation property (Made nullable based on previous fixes)

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")] // Star rating validation
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        [DataType(DataType.MultilineText)] // Hint for UI to use a textarea
        public string? Message { get; set; } // Message can be optional or required based on your needs. Made nullable here.

        // You can add more validation attributes or properties as needed
    }
}