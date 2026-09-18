namespace aractalep.Authorization
{
    public static class PermissionNames
    {
        public const string Pages = "Pages";

        // Department
        public const string Department_Read = "Department.Read";
        public const string Department_Create = "Department.Create";
        public const string Department_Update = "Department.Update";
        public const string Department_Delete = "Department.Delete";

        // VehicleRequestType
        public const string VehicleRequestType_Read = "VehicleRequestType.Read";
        public const string VehicleRequestType_Create = "VehicleRequestType.Create";
        public const string VehicleRequestType_Update = "VehicleRequestType.Update";
        public const string VehicleRequestType_Delete = "VehicleRequestType.Delete";

        // Vehicle
        public const string Vehicle_Read = "Vehicle.Read";
        public const string Vehicle_Create = "Vehicle.Create";
        public const string Vehicle_Update = "Vehicle.Update";
        public const string Vehicle_Delete = "Vehicle.Delete";

        // VehicleRequest
        public const string VehicleRequest_Read = "VehicleRequest.Read";
        public const string VehicleRequest_Create = "VehicleRequest.Create";
        public const string VehicleRequest_Update = "VehicleRequest.Update";
        public const string VehicleRequest_Delete = "VehicleRequest.Delete";
        public const string VehicleRequest_ChangeStatus = "VehicleRequest.ChangeStatus";

        // RBAC management
        public const string AppUser_Read = "AppUser.Read";
        public const string AppRole_Read = "AppRole.Read";
        public const string AppUser_Create = "AppUser.Create";
        public const string AppRole_Create = "AppRole.Create";
        public const string AppUser_Update = "AppUser.Update";
        public const string AppRole_Update = "AppRole.Update";
        public const string AppUser_Delete = "AppUser.Delete";
        public const string AppRole_Delete = "AppRole.Delete";
        public const string AppRole_AssignPermissions = "AppRole.AssignPermissions";

    }
}
