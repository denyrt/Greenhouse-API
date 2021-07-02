using System;

namespace Greenhouse.Models
{
    public class PutToGreenhouseModel
    {
        public Guid ControllerId { get; set; }
        public Guid? GreenhouseId { get; set; }
    }
}
