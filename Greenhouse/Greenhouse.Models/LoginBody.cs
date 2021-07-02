using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Models
{
    public class LoginBody
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
