using System;
using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Models.Reports
{
    public class ReportsFilter
    {
        [Required]
        public Guid GreenhouseId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
