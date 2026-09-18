using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using aractalep.Analytics.Dto;
using aractalep.Departments.Dto;

namespace aractalep.Departments
{
    public interface IDepartmentAppService : IAsyncCrudAppService<
        DepartmentDto,
        long,
        PagedDepartmentResultRequestDto,
        CreateDepartmentDto,
        DepartmentDto>
    {
        Task<DepartmentReportDto> GetReportData(long id);
    }
}
