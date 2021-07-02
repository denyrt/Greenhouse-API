using Greenhouse.Data.Models;
using Greenhouse.Models;
using Greenhouse.Models.Reports;

namespace Greenhouse.Services.Extensions
{
    public static class MappingExtensions
    {
        public static LightingControllerModel ToLightModel(this LightController controller)
        {
            return new LightingControllerModel
            {
                Id = controller.Id,
                Name = controller.Name,
                ReportIntervalSeconds = controller.ReportIntervalSeconds,
                LightingSeconds = controller.LightingTimeSeconds,
                PermissibleLightValue = controller.PermissibleLightingValue
            };
        }

        public static TemperatureControllerModel ToTemperatureModel(this TemperatureController controller) 
        {
            return new TemperatureControllerModel
            {
                Id = controller.Id,
                Name = controller.Name,
                ReportIntervalSeconds = controller.ReportIntervalSeconds,

                CelsiusToStartAeration = controller.CelsiusToStartAeration,
                CelsiusToFinishAeration = controller.CelsiusToFinishAeration,

                CelsiusToStartHeating = controller.CelsiusToStartHeating,
                CelsiusToFinishHeating = controller.CelsiusToFinishHeating,

                BottomEdgeToAlert = controller.BottomEdgeToAlert,
                UpperEdgeToAlert = controller.UpperEdgeToAlert
            };
        }

        public static WetControllerModel ToWetModel(this WetController controller)
        {
            return new WetControllerModel
            {
                Id = controller.Id,
                Name = controller.Name,
                ReportIntervalSeconds = controller.ReportIntervalSeconds,
                
                BottomWetEdge = controller.BottomWetEdge,
                UpperWetEdge = controller.UpperWetEdge                
            };
        }

        public static TemperatureReportViewModel ToReportModel(this TemperatureReport report)
        {
            return new TemperatureReportViewModel
            {
                Id = report.Id,
                Celsius = report.Celsius,
                CreateDate = report.CreateTime
            };
        }

        public static LightReportViewModel ToReportModel(this LightReport report)
        {
            return new LightReportViewModel
            {
                Id = report.Id,
                Lumens = report.Lumens,
                CreateDate = report.CreateTime
            };
        }

        public static WetReportViewModel ToReportModel(this WetReport report)
        {
            return new WetReportViewModel
            {
                Id = report.Id,
                WetPercent = report.WetPercent,
                CreateDate = report.CreateTime
            };
        }
    }
}
