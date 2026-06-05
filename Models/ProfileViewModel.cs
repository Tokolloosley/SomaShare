using System.ComponentModel.DataAnnotations;

namespace SomaShare.Models
{
    public class ProfileViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}