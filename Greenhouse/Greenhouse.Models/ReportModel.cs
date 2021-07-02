using System;

namespace Greenhouse.Models
{
    public class ReportModel
    {
        public Guid Id { get; set; }
        public double Value { get; set; }
        public DateTime CreateDate { get; set; }
    }
}