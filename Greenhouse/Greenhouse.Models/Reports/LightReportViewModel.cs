using System;

namespace Greenhouse.Models.Reports
{
    public class LightReportViewModel
    {
        public Guid Id { get; set; }
        public double Lumens { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
