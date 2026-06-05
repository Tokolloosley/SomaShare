using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SomaShare.Models
{
    public class Offer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TextbookId { get; set; }

        [Required]
        public string BuyerId { get; set; }

        [Required]
        public string SellerId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal OfferPrice { get; set; }

        [StringLength(500)]
        public string Message { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Accepted, Declined, Cancelled

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ResponseDate { get; set; }

        // Navigation properties
        [ForeignKey("TextbookId")]
        public Textbook Textbook { get; set; }

        [ForeignKey("BuyerId")]
        public ApplicationUser Buyer { get; set; }

        [ForeignKey("SellerId")]
        public ApplicationUser Seller { get; set; }

        public Transaction Transaction { get; set; }
    }
}