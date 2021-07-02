using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Models
{
    public class RegistryBody
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(55, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password not equals.")]
        public string ConfirmPassword { get; set; }
    }
}
