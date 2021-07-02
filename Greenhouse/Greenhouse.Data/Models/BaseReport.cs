using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Greenhouse.Data.Models
{
    public abstract class BaseReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        [Required]
        public Guid ControllerId { get; set; }

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    }
}
