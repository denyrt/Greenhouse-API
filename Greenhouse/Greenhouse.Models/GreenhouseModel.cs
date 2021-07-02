using System;
using System.Collections.Generic;

namespace Greenhouse.Models
{
    public class GreenhouseModel
    {       
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public int ControllersCount
        {
            get
            {
                var count = 0;
                if (TemperatureControllers != null) count += TemperatureControllers.Count;
                if (LightingControllers != null) count += LightingControllers.Count;
                if (WetControllers != null) count += WetControllers.Count;
                return count;
            }
        }

        public ICollection<TemperatureControllerModel> TemperatureControllers { get; set; } 
        
        public ICollection<LightingControllerModel> LightingControllers { get; set; }
        
        public ICollection<WetControllerModel> WetControllers { get; set; }
    }
}
