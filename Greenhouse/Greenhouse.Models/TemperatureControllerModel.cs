using System;
using System.Text.Json.Serialization;

namespace Greenhouse.Models
{
    public class TemperatureControllerModel
    {        
        public Guid? Id { get; set; }
        
        public string Name { get; set; }
        
        public double ReportIntervalSeconds { get; set; }

        public double CelsiusToStartAeration { get; set; }

        public double CelsiusToFinishAeration { get; set; }

        public double CelsiusToStartHeating { get; set; }

        public double CelsiusToFinishHeating { get; set; }

        public double BottomEdgeToAlert { get; set; }
        
        public double UpperEdgeToAlert { get; set; }
    }
}
