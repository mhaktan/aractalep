using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace aractalep.Entities
{
    // State Machine: status — Draft → PendingManagerApproval → PendingFleetApproval → Approved → Revision → Completed → Cancelled
    // Initial: Draft | Transitions: Draft→PendingManagerApproval[Submit], PendingManagerApproval→PendingFleetApproval[Approve], PendingManagerApproval→Revision[Revise], PendingFleetApproval→Approved[Approve], PendingFleetApproval→Revision[Revise], Revision→PendingManagerApproval[Resubmit], Approved→Completed[Complete], *→Cancelled[Cancel]
    [Table("VehicleRequests")]
    public class VehicleRequest : FullAuditedEntity<long>
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

        public VehicleRequestStatus Status { get; set; }

        public long DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }

        public long VehicleRequestTypeId { get; set; }

        [ForeignKey(nameof(VehicleRequestTypeId))]
        public virtual VehicleRequestType VehicleRequestType { get; set; }

        public long VehicleId { get; set; }

        [ForeignKey(nameof(VehicleId))]
        public virtual Vehicle Vehicle { get; set; }

    }
}