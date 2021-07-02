using System;
using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Models.Reports
{
    public class CreateTemperatureReportModel
    {
        [Required]
        public Guid ControllerId { get; set; }

        [Required]
        public double Celsius { get; set; }
    }
}
