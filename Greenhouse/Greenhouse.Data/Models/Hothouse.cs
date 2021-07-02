using System;
using System.Collections.Generic;

namespace Greenhouse.Data.Models
{
    public class Hothouse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<TemperatureController> TemperatureControllers { get; set; }
        public ICollection<WetController> WetControllers { get; set; }
        public ICollection<LightController> LightControllers { get; set; }
    }
}
