using System;
using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Models.Reports
{
    public class CreateWetReportModel
    {
        [Required]
        public Guid ControllerId { get; set; }
        
        [Required]
        public double WetPercent { get; set; }
    }
}