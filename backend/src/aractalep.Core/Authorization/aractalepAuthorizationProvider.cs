using Abp.Authorization;
using Abp.Localization;

namespace aractalep.Authorization
{
    public class aractalepAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull("Pages") ?? context.CreatePermission("Pages", L("Pages"));

            // Department
            pages.CreateChildPermission(PermissionNames.Department_Read, L("Department.Read"));
            pages.CreateChildPermission(PermissionNames.Department_Create, L("Department.Create"));
            pages.CreateChildPermission(PermissionNames.Department_Update, L("Department.Update"));
            pages.CreateChildPermission(PermissionNames.Department_Delete, L("Department.Delete"));

            // VehicleRequestType
            pages.CreateChildPermission(PermissionNames.VehicleRequestType_Read, L("VehicleRequestType.Read"));
            pages.CreateChildPermission(PermissionNames.VehicleRequestType_Create, L("VehicleRequestType.Create"));
            pages.CreateChildPermission(PermissionNames.VehicleRequestType_Update, L("VehicleRequestType.Update"));
            pages.CreateChildPermission(PermissionNames.VehicleRequestType_Delete, L("VehicleRequestType.Delete"));

            // Vehicle
            pages.CreateChildPermission(PermissionNames.Vehicle_Read, L("Vehicle.Read"));
            pages.CreateChildPermission(PermissionNames.Vehicle_Create, L("Vehicle.Create"));
            pages.CreateChildPermission(PermissionNames.Vehicle_Update, L("Vehicle.Update"));
            pages.CreateChildPermission(PermissionNames.Vehicle_Delete, L("Vehicle.Delete"));

            // VehicleRequest
            pages.CreateChildPermission(PermissionNames.VehicleRequest_Read, L("VehicleRequest.Read"));
            pages.CreateChildPermission(PermissionNames.VehicleRequest_Create, L("VehicleRequest.Create"));
            pages.CreateChildPermission(PermissionNames.VehicleRequest_Update, L("VehicleRequest.Update"));
            pages.CreateChildPermission(PermissionNames.VehicleRequest_Delete, L("VehicleRequest.Delete"));
            pages.CreateChildPermission(PermissionNames.VehicleRequest_ChangeStatus, L("VehicleRequest.ChangeStatus"));

            // RBAC
            pages.CreateChildPermission(PermissionNames.AppUser_Read, L("AppUser.Read"));
            pages.CreateChildPermission(PermissionNames.AppRole_Read, L("AppRole.Read"));
            pages.CreateChildPermission(PermissionNames.AppUser_Create, L("AppUser.Create"));
            pages.CreateChildPermission(PermissionNames.AppRole_Create, L("AppRole.Create"));
            pages.CreateChildPermission(PermissionNames.AppUser_Update, L("AppUser.Update"));
            pages.CreateChildPermission(PermissionNames.AppRole_Update, L("AppRole.Update"));
            pages.CreateChildPermission(PermissionNames.AppUser_Delete, L("AppUser.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_Delete, L("AppRole.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_AssignPermissions, L("AppRole.AssignPermissions"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, aractalepConsts.LocalizationSourceName);
        }
    }
}
