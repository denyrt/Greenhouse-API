using System;

namespace Greenhouse.Models.Reports
{
    public class TemperatureReportViewModel
    {
        public Guid Id { get; set; }
        public double Celsius { get; set; }
        public DateTime CreateDate { get; set; }
    }
}