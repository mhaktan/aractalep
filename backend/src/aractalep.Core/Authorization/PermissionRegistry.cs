using System.Collections.Generic;
using Abp.Dependency;

namespace aractalep.Authorization
{
    /// <summary>Single permission descriptor — name, group (entity), description.</summary>
    public class PermissionInfo
    {
        public string Name { get; }
        public string Group { get; }
        public string Description { get; }
        public bool IsRbac { get; }

        public PermissionInfo(string name, string group, string description, bool isRbac)
        {
            Name = name; Group = group; Description = description; IsRbac = isRbac;
        }
    }

    public interface IPermissionRegistry
    {
        IReadOnlyList<PermissionInfo> All { get; }
    }

    public class PermissionRegistry : IPermissionRegistry, ISingletonDependency
    {
        public IReadOnlyList<PermissionInfo> All { get; } = new List<PermissionInfo>
        {
            new PermissionInfo("Department.Read", "Department", "Read Department", false),
            new PermissionInfo("Department.Create", "Department", "Create Department", false),
            new PermissionInfo("Department.Update", "Department", "Update Department", false),
            new PermissionInfo("Department.Delete", "Department", "Delete Department", false),
            new PermissionInfo("VehicleRequestType.Read", "VehicleRequestType", "Read VehicleRequestType", false),
            new PermissionInfo("VehicleRequestType.Create", "VehicleRequestType", "Create VehicleRequestType", false),
            new PermissionInfo("VehicleRequestType.Update", "VehicleRequestType", "Update VehicleRequestType", false),
            new PermissionInfo("VehicleRequestType.Delete", "VehicleRequestType", "Delete VehicleRequestType", false),
            new PermissionInfo("Vehicle.Read", "Vehicle", "Read Vehicle", false),
            new PermissionInfo("Vehicle.Create", "Vehicle", "Create Vehicle", false),
            new PermissionInfo("Vehicle.Update", "Vehicle", "Update Vehicle", false),
            new PermissionInfo("Vehicle.Delete", "Vehicle", "Delete Vehicle", false),
            new PermissionInfo("VehicleRequest.Read", "VehicleRequest", "Read VehicleRequest", false),
            new PermissionInfo("VehicleRequest.Create", "VehicleRequest", "Create VehicleRequest", false),
            new PermissionInfo("VehicleRequest.Update", "VehicleRequest", "Update VehicleRequest", false),
            new PermissionInfo("VehicleRequest.Delete", "VehicleRequest", "Delete VehicleRequest", false),
            new PermissionInfo("VehicleRequest.ChangeStatus", "VehicleRequest", "Change VehicleRequest status", false),
            new PermissionInfo("AppUser.Read", "AppUser", "Read users", true),
            new PermissionInfo("AppRole.Read", "AppRole", "Read roles", true),
            new PermissionInfo("AppUser.Create", "AppUser", "Create users", true),
            new PermissionInfo("AppRole.Create", "AppRole", "Create roles", true),
            new PermissionInfo("AppUser.Update", "AppUser", "Update users", true),
            new PermissionInfo("AppRole.Update", "AppRole", "Update roles", true),
            new PermissionInfo("AppUser.Delete", "AppUser", "Delete users", true),
            new PermissionInfo("AppRole.Delete", "AppRole", "Delete roles", true),
            new PermissionInfo("AppRole.AssignPermissions", "AppRole", "Assign permissions to roles", true),
        };
    }
}
