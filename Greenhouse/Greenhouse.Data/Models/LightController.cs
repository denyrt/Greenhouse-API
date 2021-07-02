using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Data.Models
{
    /// <summary>
    /// Контроллер освещения.
    /// </summary>
    public class LightController : BaseController
    {
        /// <summary>
        /// Нижняя граница освещения.
        /// (Включаем освещение если значение ниже указанного.)
        /// </summary>
        [Required]
        public double PermissibleLightingValue { get; set; }  
                
        /// <summary>
        /// Время освещения.
        /// (если будет замечено низкое освещение то включаем свет на определенное время)
        /// </summary>
        [Required]
        public double LightingTimeSeconds { get; set; }

        public ICollection<LightReport> LightReports { get; set; }
    }
}