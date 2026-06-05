using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SomaShare.Models
{
    public class Textbook
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public string ISBN { get; set; }

        public string Description { get; set; }

        public string Edition { get; set; }

        public decimal AskingPrice { get; set; }

        public string Condition { get; set; }

        public string CourseCode { get; set; }

        public string ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public string SellerId { get; set; }

        public ApplicationUser Seller { get; set; }

        public DateTime ListedDate { get; set; }

        public bool IsAvailable { get; set; }

        public ICollection<Offer> Offers { get; set; }
            = new List<Offer>();
    }
}