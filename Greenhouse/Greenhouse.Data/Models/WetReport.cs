using System.ComponentModel.DataAnnotations.Schema;

namespace Greenhouse.Data.Models
{
    public class WetReport : BaseReport
    {
        public double WetPercent { get; set; }

        [ForeignKey("ControllerId")]
        public WetController WetController { get; set; }
    }
}