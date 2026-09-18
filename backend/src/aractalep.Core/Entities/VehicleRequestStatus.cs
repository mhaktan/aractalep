namespace aractalep.Entities
{
    public enum VehicleRequestStatus
    {
        Draft = 0,
        PendingManagerApproval = 1,
        PendingFleetApproval = 2,
        Approved = 3,
        Revision = 4,
        Completed = 5,
        Cancelled = 6,
    }
}