using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Data.Models
{
    /// <summary>
    /// Контроллер температуры.
    /// </summary>
    public class TemperatureController : BaseController
    {
        /// <summary>
        /// Температура для включения проветривания.
        /// </summary>
        [Required]
        public double CelsiusToStartAeration { get; set; }      

        /// <summary>
        /// Температура для отключения проветривания.
        /// </summary>
        [Required]
        public double CelsiusToFinishAeration { get; set; }

        /// <summary>
        /// Температура для начала подогрева.
        /// </summary>
        [Required]
        public double CelsiusToStartHeating { get; set; }

        /// <summary>
        /// Температура для отключения подогрева.
        /// </summary>
        [Required]
        public double CelsiusToFinishHeating { get; set; }

        /// <summary>
        /// Нижняя граница температуры для предупреждения.
        /// </summary>
        [Required]
        public double BottomEdgeToAlert { get; set; }

        /// <summary>
        /// Верхняя граница температуры для предупреждения.
        /// </summary>
        [Required]
        public double UpperEdgeToAlert { get; set; }

        public ICollection<TemperatureReport> TemperatureReports { get; set; }
    }
}
