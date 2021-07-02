using System.ComponentModel.DataAnnotations.Schema;

namespace Greenhouse.Data.Models
{
    public class TemperatureReport : BaseReport
    {
        public double Celsius { get; set; }

        [ForeignKey("ControllerId")]
        public TemperatureController TemperatureController { get; set; }
    }
}
