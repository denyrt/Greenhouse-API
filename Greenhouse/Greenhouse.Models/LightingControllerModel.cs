using System;

namespace Greenhouse.Models
{
    public class LightingControllerModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public double ReportIntervalSeconds { get; set; }
        public double PermissibleLightValue { get; set; }
        public double LightingSeconds {get;set;}
    }
}
