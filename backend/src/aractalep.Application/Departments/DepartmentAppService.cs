using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using aractalep.Entities;
using aractalep.Departments.Dto;
using aractalep.Analytics.Dto;
using aractalep.VehicleRequests.Dto;
using aractalep.Approvals.Dto;
using aractalep.Authorization;
using aractalep.Flows;

namespace aractalep.Departments
{
    public class DepartmentAppService : AsyncCrudAppService<
        Department,
        DepartmentDto,
        long,
        PagedDepartmentResultRequestDto,
        CreateDepartmentDto,
        DepartmentDto>,
        IDepartmentAppService
    {
        private readonly IFlowEngine _flowEngine;

        public DepartmentAppService(IRepository<Department, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Department_Read;
            GetAllPermissionName = PermissionNames.Department_Read;
            CreatePermissionName = PermissionNames.Department_Create;
            UpdatePermissionName = PermissionNames.Department_Update;
            DeletePermissionName = PermissionNames.Department_Delete;
        }

        protected override IQueryable<Department> CreateFilteredQuery(PagedDepartmentResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name));
        }

        public override async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Department", result);
            return result;
        }

        public override async Task<DepartmentDto> UpdateAsync(DepartmentDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Department", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Department", new { Id = input.Id });
        }
        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Department_Read)]
        public async Task<DepartmentReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.VehicleRequests)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new DepartmentReportDto
            {
                Data = ObjectMapper.Map<DepartmentDto>(root),
                VehicleRequests = ObjectMapper.Map<List<VehicleRequestDto>>(
                    root.VehicleRequests == null ? new List<VehicleRequest>() : root.VehicleRequests.ToList()),
            };
        }

    }
}
