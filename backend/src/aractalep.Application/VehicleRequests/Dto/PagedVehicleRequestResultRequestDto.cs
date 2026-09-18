using System;
using Abp.Application.Services.Dto;

namespace aractalep.VehicleRequests.Dto
{
    public class PagedVehicleRequestResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? DepartmentId { get; set; }
        public long? VehicleRequestTypeId { get; set; }
        public long? VehicleId { get; set; }
        public string RequestNo { get; set; }
        public long? RequestTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Destination { get; set; }
        public string Purpose { get; set; }
        public string DriverName { get; set; }
        public bool? IsPoolExternal { get; set; }
        public string ExternalVehicleInfo { get; set; }
        public string FleetNote { get; set; }
        public string RevisionNote { get; set; }
        public int? Status { get; set; }
        public long? RequestTypeIdFrom { get; set; }
        public long? RequestTypeIdTo { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
