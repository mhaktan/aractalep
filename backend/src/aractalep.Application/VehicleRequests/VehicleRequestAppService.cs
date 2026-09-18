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
using aractalep.VehicleRequests.Dto;
using aractalep.Analytics.Dto;
using aractalep.StateMachine.Dto;
using aractalep.Authorization;
using aractalep.Flows;

namespace aractalep.VehicleRequests
{
    public class VehicleRequestAppService : AsyncCrudAppService<
        VehicleRequest,
        VehicleRequestDto,
        long,
        PagedVehicleRequestResultRequestDto,
        CreateVehicleRequestDto,
        VehicleRequestDto>,
        IVehicleRequestAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public VehicleRequestAppService(IRepository<VehicleRequest, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.VehicleRequest_Read;
            GetAllPermissionName = PermissionNames.VehicleRequest_Read;
            CreatePermissionName = PermissionNames.VehicleRequest_Create;
            UpdatePermissionName = PermissionNames.VehicleRequest_Update;
            DeletePermissionName = PermissionNames.VehicleRequest_Delete;
        }

        protected override IQueryable<VehicleRequest> CreateFilteredQuery(PagedVehicleRequestResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.RequestNo != null && x.RequestNo.Contains(input.Keyword)) ||
                    (x.Destination != null && x.Destination.Contains(input.Keyword)) ||
                    (x.Purpose != null && x.Purpose.Contains(input.Keyword)) ||
                    (x.DriverName != null && x.DriverName.Contains(input.Keyword)) ||
                    (x.ExternalVehicleInfo != null && x.ExternalVehicleInfo.Contains(input.Keyword)) ||
                    (x.FleetNote != null && x.FleetNote.Contains(input.Keyword)) ||
                    (x.RevisionNote != null && x.RevisionNote.Contains(input.Keyword)))
                .WhereIf(!input.RequestNo.IsNullOrWhiteSpace(), x => x.RequestNo != null && x.RequestNo.Contains(input.RequestNo))
                .WhereIf(!input.Destination.IsNullOrWhiteSpace(), x => x.Destination != null && x.Destination.Contains(input.Destination))
                .WhereIf(!input.Purpose.IsNullOrWhiteSpace(), x => x.Purpose != null && x.Purpose.Contains(input.Purpose))
                .WhereIf(!input.DriverName.IsNullOrWhiteSpace(), x => x.DriverName != null && x.DriverName.Contains(input.DriverName))
                .WhereIf(!input.ExternalVehicleInfo.IsNullOrWhiteSpace(), x => x.ExternalVehicleInfo != null && x.ExternalVehicleInfo.Contains(input.ExternalVehicleInfo))
                .WhereIf(!input.FleetNote.IsNullOrWhiteSpace(), x => x.FleetNote != null && x.FleetNote.Contains(input.FleetNote))
                .WhereIf(!input.RevisionNote.IsNullOrWhiteSpace(), x => x.RevisionNote != null && x.RevisionNote.Contains(input.RevisionNote))
                .WhereIf(input.RequestTypeId.HasValue, x => x.RequestTypeId == input.RequestTypeId.Value)
                .WhereIf(input.StartDate.HasValue, x => x.StartDate == input.StartDate.Value)
                .WhereIf(input.EndDate.HasValue, x => x.EndDate == input.EndDate.Value)
                .WhereIf(input.IsPoolExternal.HasValue, x => x.IsPoolExternal == input.IsPoolExternal.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (VehicleRequestStatus)input.Status.Value)
                .WhereIf(input.RequestTypeIdFrom.HasValue, x => x.RequestTypeId >= input.RequestTypeIdFrom.Value)
                .WhereIf(input.RequestTypeIdTo.HasValue, x => x.RequestTypeId <= input.RequestTypeIdTo.Value)
                .WhereIf(input.StartDateFrom.HasValue, x => x.StartDate >= input.StartDateFrom.Value)
                .WhereIf(input.StartDateTo.HasValue, x => x.StartDate <= input.StartDateTo.Value)
                .WhereIf(input.EndDateFrom.HasValue, x => x.EndDate >= input.EndDateFrom.Value)
                .WhereIf(input.EndDateTo.HasValue, x => x.EndDate <= input.EndDateTo.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (VehicleRequestStatus)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (VehicleRequestStatus)input.StatusNot.Value)
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId == input.DepartmentId.Value)
                .WhereIf(input.VehicleRequestTypeId.HasValue, x => x.VehicleRequestTypeId == input.VehicleRequestTypeId.Value)
                .WhereIf(input.VehicleId.HasValue, x => x.VehicleId == input.VehicleId.Value);
        }

        public override async Task<VehicleRequestDto> CreateAsync(CreateVehicleRequestDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "VehicleRequest", result);

            // Frontend creates records with status pre-set without going through ChangeStatusAsync,
            // so mirror on-field-change here whenever the initial status isn't the default. Otherwise
            // status-driven flows (e.g. approval) never fire on plain Create.
            if (result.Status != (int)VehicleRequestStatus.Draft)
                await _flowEngine.TriggerAsync("on-field-change", "VehicleRequest", result);
            return result;
        }

        public override async Task<VehicleRequestDto> UpdateAsync(VehicleRequestDto input)
        {
            // State machine: validate status transition + log
            var existing = await Repository.GetAsync(input.Id);
            var statusChanged = (int)existing.Status != input.Status;
            if (statusChanged)
            {
                var fromStatus = existing.Status.ToString();
                var toStatus = ((VehicleRequestStatus)input.Status).ToString();
                ValidateStatusTransition(existing.Status, (VehicleRequestStatus)input.Status);

                // Log status change
                await _statusChangeLogRepo.InsertAsync(new StatusChangeLog
                {
                    EntityType = "VehicleRequest",
                    EntityId = input.Id.ToString(),
                    FromStatus = fromStatus,
                    ToStatus = toStatus,
                    Action = "Update",
                    ChangedByUserId = AbpSession.UserId
                });
            }

            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "VehicleRequest", result);

            // Frontend updates status via plain UpdateAsync (not ChangeStatusAsync) — fire
            // on-field-change so status-driven flows pick up the transition.
            if (statusChanged)
                await _flowEngine.TriggerAsync("on-field-change", "VehicleRequest", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "VehicleRequest", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.VehicleRequest_ChangeStatus, PermissionNames.VehicleRequest_Update, RequireAllPermissions = false)]
        public async Task<VehicleRequestDto> ChangeStatusAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            // Find valid transition
            var transitions = new (string From, string To, string Action, bool Readonly)[]
            {
            ("Draft", "PendingManagerApproval", "Submit", false),
            ("PendingManagerApproval", "PendingFleetApproval", "Approve", false),
            ("PendingManagerApproval", "Revision", "Revise", false),
            ("PendingFleetApproval", "Approved", "Approve", false),
            ("PendingFleetApproval", "Revision", "Revise", false),
            ("Revision", "PendingManagerApproval", "Resubmit", false),
            ("Approved", "Completed", "Complete", false),
            ("*", "Cancelled", "Cancel", false)
            };

            var transition = transitions.FirstOrDefault(t =>
                (t.From == "*" || t.From == currentStatus) && t.Action == input.Action);

            if (transition == default)
                throw new Abp.UI.UserFriendlyException($"Invalid action '{input.Action}' from status '{currentStatus}'");

            // Onay ekranindan gelen serbest metni gecisin zorunlu alanlarina tasi
            if (input.ActionData == null) input.ActionData = new Dictionary<string, string>();
            var genericNote = input.ActionData.ContainsKey("comment") && !string.IsNullOrWhiteSpace(input.ActionData["comment"])
                ? input.ActionData["comment"]
                : (input.ActionData.ContainsKey("revisionNote") ? input.ActionData["revisionNote"] : null);
            if (!string.IsNullOrWhiteSpace(genericNote))
            {
                foreach (var rf in new[] { "revisionNote" })
                {
                    if (!input.ActionData.ContainsKey(rf) || string.IsNullOrWhiteSpace(input.ActionData[rf]))
                        input.ActionData[rf] = genericNote;
                }
            }
            // Validate required fields per transition
            if (input.Action == "Revise" && (input.ActionData == null || !input.ActionData.ContainsKey("revisionNote") || string.IsNullOrWhiteSpace(input.ActionData["revisionNote"])))
                throw new Abp.UI.UserFriendlyException("Revise requires: revisionNote");
            // Bu gecisler icin bagli kayit on kosulu yok

            var fromStatus = currentStatus;

            // Apply new status
            entity.Status = (VehicleRequestStatus)Enum.Parse(typeof(VehicleRequestStatus), transition.To);
            if (input.ActionData != null && input.ActionData.ContainsKey("revisionNote") && !string.IsNullOrWhiteSpace(input.ActionData["revisionNote"]))
                entity.RevisionNote = input.ActionData["revisionNote"];

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "VehicleRequest" && a.EntityId == id.ToString() && a.Status == "Pending")
                    .ToList();
                foreach (var pendingRec in pending)
                {
                    pendingRec.Status = "Cancelled";
                    pendingRec.ActionTaken = "Cancel";
                    pendingRec.ActionDate = DateTime.UtcNow;
                    pendingRec.Comment = "Entity cancelled by submitter.";
                    await _approvalRepo.UpdateAsync(pendingRec);
                }
            }

            // Log status change
            await _statusChangeLogRepo.InsertAsync(new Entities.StatusChangeLog
            {
                EntityType = "VehicleRequest",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "VehicleRequest", result);

            return result;
        }

        private void ValidateStatusTransition(VehicleRequestStatus from, VehicleRequestStatus to)
        {
            var allowed = new (string From, string To)[]
            {
                ("Draft", "PendingManagerApproval"),
                ("PendingManagerApproval", "PendingFleetApproval"),
                ("PendingManagerApproval", "Revision"),
                ("PendingFleetApproval", "Approved"),
                ("PendingFleetApproval", "Revision"),
                ("Revision", "PendingManagerApproval"),
                ("Approved", "Completed"),
                ("*", "Cancelled")
            };

            var isValid = allowed.Any(t =>
                (t.From == "*" || t.From == from.ToString()) &&
                t.To == to.ToString());

            if (!isValid)
                throw new Abp.UI.UserFriendlyException($"Invalid status transition from {from} to {to}");
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.VehicleRequest_Read)]
        public List<GroupCountDto> GetGroupedCount(VehicleRequestGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Status", "DepartmentId", "VehicleRequestTypeId", "VehicleId" };
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
                case "DepartmentId":
                    return query
                        .GroupBy(x => new { Key = x.DepartmentId, Label = x.Department == null ? null : x.Department.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "VehicleRequestTypeId":
                    return query
                        .GroupBy(x => new { Key = x.VehicleRequestTypeId, Label = x.VehicleRequestType == null ? null : x.VehicleRequestType.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "VehicleId":
                    return query
                        .GroupBy(x => new { Key = x.VehicleId, Label = x.Vehicle == null ? null : x.Vehicle.Plate })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.VehicleRequest_Read)]
        public decimal? GetStats(VehicleRequestStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "StartDate", "EndDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    case "StartDate|EndDate":
                    {
                        var pairsStartDateEndDate = query
                            .Select(x => new { A = x.StartDate, B = x.EndDate })
                            .ToList();
                        if (pairsStartDateEndDate.Count == 0) return null;
                        return (decimal)pairsStartDateEndDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "EndDate|StartDate":
                    {
                        var pairsEndDateStartDate = query
                            .Select(x => new { A = x.EndDate, B = x.StartDate })
                            .ToList();
                        if (pairsEndDateStartDate.Count == 0) return null;
                        return (decimal)pairsEndDateStartDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "RequestTypeId" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "RequestTypeId": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.RequestTypeId)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.RequestTypeId)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.RequestTypeId)
                            : query.Average(x => (decimal?)x.RequestTypeId);
                        default: return null;
            }
        }

    }
}
