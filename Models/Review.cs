using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SomaShare.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TransactionId { get; set; }

        [Required]
        public string ReviewerUserId { get; set; }

        [Required]
        public string ReviewedUserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("TransactionId")]
        public Transaction Transaction { get; set; }

        [ForeignKey("ReviewerUserId")]
        public ApplicationUser ReviewerUser { get; set; }

        [ForeignKey("ReviewedUserId")]
        public ApplicationUser ReviewedUser { get; set; }
    }
}