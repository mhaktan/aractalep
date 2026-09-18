using Microsoft.EntityFrameworkCore;
using Abp.EntityFrameworkCore;
using aractalep.Entities;

namespace aractalep.EntityFrameworkCore
{
    public class aractalepDbContext : AbpDbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<VehicleRequestType> VehicleRequestTypes { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleRequest> VehicleRequests { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ApprovalRecord> ApprovalRecords { get; set; }
        public DbSet<StatusChangeLog> StatusChangeLogs { get; set; }


        public aractalepDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Department 1:N VehicleRequest
            modelBuilder.Entity<VehicleRequest>()
                .HasOne(x => x.Department)
                .WithMany(x => x.VehicleRequests)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // VehicleRequestType 1:N VehicleRequest
            modelBuilder.Entity<VehicleRequest>()
                .HasOne(x => x.VehicleRequestType)
                .WithMany(x => x.VehicleRequests)
                .HasForeignKey(x => x.VehicleRequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle 1:N VehicleRequest
            modelBuilder.Entity<VehicleRequest>()
                .HasOne(x => x.Vehicle)
                .WithMany(x => x.VehicleRequests)
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);


            // RBAC: AppUser N:N AppRole via UserRole junction
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // RolePermission: AppRole 1:N RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionName })
                .IsUnique();

            // AppRole.Name unique
            modelBuilder.Entity<AppRole>()
                .HasIndex(r => r.Name)
                .IsUnique();

        }
    }
}
