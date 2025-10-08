using System.ComponentModel.DataAnnotations;

namespace Authentication.Models
{
    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; }  = string.Empty;

        [Required]
        public string Password { get; set; }  = string.Empty;

        [Required]
        public DateTime Birthday { get; set; }  

        [Required]
        public string Gender { get; set; }  = string.Empty;
    }
}
