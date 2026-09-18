using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace aractalep.Vehicles.Dto
{
    [AutoMapTo(typeof(Entities.Vehicle))]
    public class CreateVehicleDto
    {
        [Required]
        [MaxLength(20)]
        public string Plate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Brand { get; set; }

        [Required]
        [MaxLength(100)]
        public string Model { get; set; }

        public int? Year { get; set; }

        public int? Capacity { get; set; }

        public int Status { get; set; }

    }
}