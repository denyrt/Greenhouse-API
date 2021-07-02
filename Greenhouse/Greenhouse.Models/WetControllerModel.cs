using System;

namespace Greenhouse.Models
{
    public class WetControllerModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public double ReportIntervalSeconds { get; set; }
        public double BottomWetEdge { get; set; }
        public double UpperWetEdge { get; set; }
    }
}
