using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace aractalep.VehicleRequestTypes.Dto
{
    [AutoMapTo(typeof(Entities.VehicleRequestType))]
    public class CreateVehicleRequestTypeDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

    }
}