using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Models
{
    public class UpdateGreenhouseModel
    {
        [Required]
        public string Name { get; set; }
    }
}
