using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SomaShare.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OfferId { get; set; }

        [Required]
        public string BuyerId { get; set; }

        [Required]
        public string SellerId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal FinalPrice { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Completed"; // Pending, Completed, Cancelled

        // Navigation properties
        [ForeignKey("OfferId")]
        public Offer Offer { get; set; }

        [ForeignKey("BuyerId")]
        public ApplicationUser Buyer { get; set; }

        [ForeignKey("SellerId")]
        public ApplicationUser Seller { get; set; }

        public Review Review { get; set; }
    }
}