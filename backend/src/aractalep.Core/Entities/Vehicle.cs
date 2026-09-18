using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace aractalep.Entities
{
    [Table("Vehicles")]
    public class Vehicle : FullAuditedEntity<long>
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

        public VehicleStatus Status { get; set; }

        public virtual ICollection<VehicleRequest> VehicleRequests { get; set; }

    }
}