using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace aractalep.Entities
{
    [Table("Departments")]
    public class Department : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public virtual ICollection<VehicleRequest> VehicleRequests { get; set; }

    }
}