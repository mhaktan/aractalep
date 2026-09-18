using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using aractalep.Analytics.Dto;
using aractalep.VehicleRequestTypes.Dto;

namespace aractalep.VehicleRequestTypes
{
    public interface IVehicleRequestTypeAppService : IAsyncCrudAppService<
        VehicleRequestTypeDto,
        long,
        PagedVehicleRequestTypeResultRequestDto,
        CreateVehicleRequestTypeDto,
        VehicleRequestTypeDto>
    {
        Task<VehicleRequestTypeReportDto> GetReportData(long id);
    }
}
