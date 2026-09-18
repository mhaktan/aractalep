using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using aractalep.Analytics.Dto;
using aractalep.Vehicles.Dto;

namespace aractalep.Vehicles
{
    public interface IVehicleAppService : IAsyncCrudAppService<
        VehicleDto,
        long,
        PagedVehicleResultRequestDto,
        CreateVehicleDto,
        VehicleDto>
    {
        List<GroupCountDto> GetGroupedCount(VehicleGroupedCountInput input);
        decimal? GetStats(VehicleStatsInput input);
        Task<VehicleReportDto> GetReportData(long id);
    }
}
