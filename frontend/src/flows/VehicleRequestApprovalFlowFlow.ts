// Auto-generated flow: VehicleRequest Approval Flow
// Auto-generated approval flow for VehicleRequest. Customize email templates and add conditions as needed.
// Resource: VehicleRequest
// Enabled: true
//
// Nodes:
  // trigger: On VehicleRequest Submit
  // condition: Status = PendingManagerApproval?
  // approval: VehicleRequest Approval
  // action: Send Approval Email
  // trigger: On VehicleRequest Approved
  // action: Send Completion Email
//
// Edges:
  // On VehicleRequest Submit → Status = PendingManagerApproval?
  // Status = PendingManagerApproval? → VehicleRequest Approval (true)
  // VehicleRequest Approval → Send Approval Email
  // On VehicleRequest Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_VEHICLEREQUEST_APPROVAL_FLOW_ID = 'flow-VehicleRequest-approval';
