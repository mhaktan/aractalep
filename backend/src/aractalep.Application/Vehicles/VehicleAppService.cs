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
using aractalep.Vehicles.Dto;
using aractalep.Analytics.Dto;
using aractalep.VehicleRequests.Dto;
using aractalep.Approvals.Dto;
using aractalep.Authorization;
using aractalep.Flows;

namespace aractalep.Vehicles
{
    public class VehicleAppService : AsyncCrudAppService<
        Vehicle,
        VehicleDto,
        long,
        PagedVehicleResultRequestDto,
        CreateVehicleDto,
        VehicleDto>,
        IVehicleAppService
    {
        private readonly IFlowEngine _flowEngine;

        public VehicleAppService(IRepository<Vehicle, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Vehicle_Read;
            GetAllPermissionName = PermissionNames.Vehicle_Read;
            CreatePermissionName = PermissionNames.Vehicle_Create;
            UpdatePermissionName = PermissionNames.Vehicle_Update;
            DeletePermissionName = PermissionNames.Vehicle_Delete;
        }

        protected override IQueryable<Vehicle> CreateFilteredQuery(PagedVehicleResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Plate != null && x.Plate.Contains(input.Keyword)) ||
                    (x.Brand != null && x.Brand.Contains(input.Keyword)) ||
                    (x.Model != null && x.Model.Contains(input.Keyword)))
                .WhereIf(!input.Plate.IsNullOrWhiteSpace(), x => x.Plate != null && x.Plate.Contains(input.Plate))
                .WhereIf(!input.Brand.IsNullOrWhiteSpace(), x => x.Brand != null && x.Brand.Contains(input.Brand))
                .WhereIf(!input.Model.IsNullOrWhiteSpace(), x => x.Model != null && x.Model.Contains(input.Model))
                .WhereIf(input.Year.HasValue, x => x.Year == input.Year.Value)
                .WhereIf(input.Capacity.HasValue, x => x.Capacity == input.Capacity.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (VehicleStatus)input.Status.Value)
                .WhereIf(input.YearFrom.HasValue, x => x.Year >= input.YearFrom.Value)
                .WhereIf(input.YearTo.HasValue, x => x.Year <= input.YearTo.Value)
                .WhereIf(input.CapacityFrom.HasValue, x => x.Capacity >= input.CapacityFrom.Value)
                .WhereIf(input.CapacityTo.HasValue, x => x.Capacity <= input.CapacityTo.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (VehicleStatus)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (VehicleStatus)input.StatusNot.Value);
        }

        public override async Task<VehicleDto> CreateAsync(CreateVehicleDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Vehicle", result);
            return result;
        }

        public override async Task<VehicleDto> UpdateAsync(VehicleDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Vehicle", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Vehicle", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Vehicle_Read)]
        public List<GroupCountDto> GetGroupedCount(VehicleGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Status" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "Status":
                    return query
                        .GroupBy(x => x.Status)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.Vehicle_Read)]
        public decimal? GetStats(VehicleStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new string[0];
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "Year", "Capacity" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "Year": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Year)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Year)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Year)
                            : query.Average(x => (decimal?)x.Year);
                        case "Capacity": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Capacity)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Capacity)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Capacity)
                            : query.Average(x => (decimal?)x.Capacity);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Vehicle_Read)]
        public async Task<VehicleReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.VehicleRequests)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new VehicleReportDto
            {
                Data = ObjectMapper.Map<VehicleDto>(root),
                VehicleRequests = ObjectMapper.Map<List<VehicleRequestDto>>(
                    root.VehicleRequests == null ? new List<VehicleRequest>() : root.VehicleRequests.ToList()),
            };
        }

    }
}
