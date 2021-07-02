using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Greenhouse.Data.Models
{
    public abstract class BaseController
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double ReportIntervalSeconds { get; set; } = TimeSpan.FromMinutes(5).TotalSeconds;  
        
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }
        public Guid? GreenhouseId { get; set; }
        public Hothouse Greenhouse { get; set; }
    }
}
