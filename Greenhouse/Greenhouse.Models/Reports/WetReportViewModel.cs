using System;

namespace Greenhouse.Models.Reports
{
    public class WetReportViewModel
    {
        public Guid Id { get; set; }
        public double WetPercent { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
