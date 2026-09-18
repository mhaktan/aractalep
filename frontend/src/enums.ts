// ---------------------------------------------------------------------------
// Enum definitions — auto-generated from ER model
// Maps integer values to display labels for enum fields
// ---------------------------------------------------------------------------

// Vehicle.status
export const VehicleStatusMap: Record<string, string> = {
  '0': 'Available',
  '1': 'InUse',
  '2': 'UnderMaintenance',
  '3': 'OutOfService'
};
export const VehicleStatusOptions = [
  { label: 'Available', value: '0' },
  { label: 'InUse', value: '1' },
  { label: 'UnderMaintenance', value: '2' },
  { label: 'OutOfService', value: '3' }
];

// VehicleRequest.status
export const VehicleRequestStatusMap: Record<string, string> = {
  '0': 'Draft',
  '1': 'PendingManagerApproval',
  '2': 'PendingFleetApproval',
  '3': 'Approved',
  '4': 'Revision',
  '5': 'Completed',
  '6': 'Cancelled'
};
export const VehicleRequestStatusOptions = [
  { label: 'Draft', value: '0' },
  { label: 'PendingManagerApproval', value: '1' },
  { label: 'PendingFleetApproval', value: '2' },
  { label: 'Approved', value: '3' },
  { label: 'Revision', value: '4' },
  { label: 'Completed', value: '5' },
  { label: 'Cancelled', value: '6' }
];
