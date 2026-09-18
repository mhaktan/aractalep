using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace aractalep.VehicleRequestTypes.Dto
{
    [AutoMapFrom(typeof(Entities.VehicleRequestType))]
    public class VehicleRequestTypeDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}