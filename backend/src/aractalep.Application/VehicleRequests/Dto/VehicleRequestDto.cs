using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace aractalep.VehicleRequests.Dto
{
    [AutoMapFrom(typeof(Entities.VehicleRequest))]
    public class VehicleRequestDto : EntityDto<long>
    {
        public string RequestNo { get; set; }

        public long RequestTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Destination { get; set; }

        public string Purpose { get; set; }

        public string DriverName { get; set; }

        public bool IsPoolExternal { get; set; }

        public string ExternalVehicleInfo { get; set; }

        public string FleetNote { get; set; }

        public string RevisionNote { get; set; }

        public int Status { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long DepartmentId { get; set; }

        public long VehicleRequestTypeId { get; set; }

        public long VehicleId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}