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
using aractalep.VehicleRequestTypes.Dto;
using aractalep.Analytics.Dto;
using aractalep.VehicleRequests.Dto;
using aractalep.Approvals.Dto;
using aractalep.Authorization;
using aractalep.Flows;

namespace aractalep.VehicleRequestTypes
{
    public class VehicleRequestTypeAppService : AsyncCrudAppService<
        VehicleRequestType,
        VehicleRequestTypeDto,
        long,
        PagedVehicleRequestTypeResultRequestDto,
        CreateVehicleRequestTypeDto,
        VehicleRequestTypeDto>,
        IVehicleRequestTypeAppService
    {
        private readonly IFlowEngine _flowEngine;

        public VehicleRequestTypeAppService(IRepository<VehicleRequestType, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.VehicleRequestType_Read;
            GetAllPermissionName = PermissionNames.VehicleRequestType_Read;
            CreatePermissionName = PermissionNames.VehicleRequestType_Create;
            UpdatePermissionName = PermissionNames.VehicleRequestType_Update;
            DeletePermissionName = PermissionNames.VehicleRequestType_Delete;
        }

        protected override IQueryable<VehicleRequestType> CreateFilteredQuery(PagedVehicleRequestTypeResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description));
        }

        public override async Task<VehicleRequestTypeDto> CreateAsync(CreateVehicleRequestTypeDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "VehicleRequestType", result);
            return result;
        }

        public override async Task<VehicleRequestTypeDto> UpdateAsync(VehicleRequestTypeDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "VehicleRequestType", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "VehicleRequestType", new { Id = input.Id });
        }
        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.VehicleRequestType_Read)]
        public async Task<VehicleRequestTypeReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.VehicleRequests)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new VehicleRequestTypeReportDto
            {
                Data = ObjectMapper.Map<VehicleRequestTypeDto>(root),
                VehicleRequests = ObjectMapper.Map<List<VehicleRequestDto>>(
                    root.VehicleRequests == null ? new List<VehicleRequest>() : root.VehicleRequests.ToList()),
            };
        }

    }
}
