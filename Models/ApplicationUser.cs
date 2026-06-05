using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace SomaShare.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public double AverageRating { get; set; } = 0;
        public double TrustScore
        {
            get
            {
                return (AverageRating * 0.7) +
                       (TotalTransactions * 0.3);
            }
        }

        public int TotalTransactions { get; set; } = 0;

        // Navigation properties
        public ICollection<Textbook> TextbooksListed { get; set; } = new List<Textbook>();
        public ICollection<Offer> OffersPlaced { get; set; } = new List<Offer>();
        public ICollection<Offer> OffersReceived { get; set; } = new List<Offer>();
        public ICollection<Transaction> PurchaseTransactions { get; set; } = new List<Transaction>();
        public ICollection<Transaction> SaleTransactions { get; set; } = new List<Transaction>();
        public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
        public ICollection<Review> ReviewsGiven { get; set; } = new List<Review>();
        public ICollection<WantedAd> WantedAds { get; set; } = new List<WantedAd>();
    }
}