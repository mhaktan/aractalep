using System;
using aractalep.Analytics.Dto;

namespace aractalep.Vehicles.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class VehicleGroupedCountInput : PagedVehicleResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class VehicleStatsInput : PagedVehicleResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
