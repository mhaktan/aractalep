using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace aractalep.Vehicles.Dto
{
    [AutoMapFrom(typeof(Entities.Vehicle))]
    public class VehicleDto : EntityDto<long>
    {
        public string Plate { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int? Year { get; set; }

        public int? Capacity { get; set; }

        public int Status { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}