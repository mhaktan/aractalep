using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using aractalep.Analytics.Dto;
using aractalep.StateMachine.Dto;
using aractalep.VehicleRequests.Dto;

namespace aractalep.VehicleRequests
{
    public interface IVehicleRequestAppService : IAsyncCrudAppService<
        VehicleRequestDto,
        long,
        PagedVehicleRequestResultRequestDto,
        CreateVehicleRequestDto,
        VehicleRequestDto>
    {
        Task<VehicleRequestDto> ChangeStatusAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(VehicleRequestGroupedCountInput input);
        decimal? GetStats(VehicleRequestStatsInput input);
    }
}
