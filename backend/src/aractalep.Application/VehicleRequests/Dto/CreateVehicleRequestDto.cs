using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace aractalep.VehicleRequests.Dto
{
    [AutoMapTo(typeof(Entities.VehicleRequest))]
    public class CreateVehicleRequestDto
    {
        [Required]
        [MaxLength(50)]
        public string RequestNo { get; set; }

        public long RequestTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [MaxLength(500)]
        public string Destination { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Purpose { get; set; }

        [MaxLength(200)]
        public string DriverName { get; set; }

        public bool IsPoolExternal { get; set; }

        [MaxLength(500)]
        public string ExternalVehicleInfo { get; set; }

        [MaxLength(1000)]
        public string FleetNote { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public int Status { get; set; }

        public long DepartmentId { get; set; }

        public long VehicleRequestTypeId { get; set; }

        public long VehicleId { get; set; }

    }
}