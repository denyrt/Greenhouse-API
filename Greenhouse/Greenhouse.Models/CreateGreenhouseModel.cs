using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Models
{
    public class CreateGreenhouseModel
    {
        [Required]
        public string Name { get; set; }
    }
}
