using System;
using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Models.Reports
{
    public class CreateLightReportModel
    {
        [Required]
        public Guid ControllerId { get; set; }
        
        [Required]
        public double Lumens { get; set; }
    }
}
