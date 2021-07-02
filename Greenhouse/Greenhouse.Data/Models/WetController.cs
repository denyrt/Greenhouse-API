using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Data.Models
{
    /// <summary>
    /// Контроллер влажности.
    /// </summary>
    public class WetController : BaseController
    {
        /// <summary>
        /// Нижняя граница влажности для включения полива.
        /// </summary>
        [Required]
        public double BottomWetEdge { get; set; }

        /// <summary>
        /// Верхняя граница влажности для отключения полива.
        /// </summary>
        [Required]
        public double UpperWetEdge { get; set; }

        public ICollection<WetReport> WetReports { get; set; }
    }
}