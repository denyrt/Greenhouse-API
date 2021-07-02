using System.ComponentModel.DataAnnotations.Schema;

namespace Greenhouse.Data.Models
{
    public class LightReport : BaseReport
    {
        public double Lumens { get; set; }

        [ForeignKey("ControllerId")]
        public LightController LightController { get; set; }
    }
}