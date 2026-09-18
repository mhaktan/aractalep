using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using aractalep.EntityFrameworkCore;
using aractalep.EntityFrameworkCore.Seed;
using aractalep.Entities;

namespace aractalep.Web.Host
{
    /// <summary>
    /// Background service that runs migration + seed once at startup
    /// without blocking the HTTP pipeline.
    /// </summary>
    public class MigrationHostedService : IHostedService
    {
        private readonly IConfiguration _config;

        public MigrationHostedService(IConfiguration config)
        {
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var connStr = _config.GetConnectionString("Default") ?? "";
            if (string.IsNullOrEmpty(connStr)) return;

            // Run in background to avoid blocking host startup (prevents EF tooling timeout)
            _ = Task.Run(async () =>
            {
                // Small delay to ensure host is fully started before DB operations
                await Task.Delay(1000, cancellationToken);
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder();
                    optionsBuilder.UseNpgsql(connStr);

                    using (var db = new aractalepDbContext(optionsBuilder.Options))
                    {
                        // EnsureCreated bos veritabaninda en guncel semayi kurar ve true doner.
                        // Dolu veritabaninda hicbir sey yapmaz — o durumda bekleyen migration'lar
                        // SchemaMigrations.Apply icinde sirayla calistirilir.
                        var freshlyCreated = db.Database.EnsureCreated();
                        Console.WriteLine("[Migration] Migration tanimi yok — sema EnsureCreated ile yonetiliyor.");
                        Console.WriteLine("[Migration] Database is up to date.");

                        // Seed sample data — wrapped in its own try so a failure here doesn't block RBAC seed below.
                        try
                        {
                    if (!db.Departments.Any())
                    {
                        db.Departments.AddRange(
                    new Department { Id = 1, Name = "Birim 1" },
                    new Department { Id = 2, Name = "Birim 2" }
                        );
                    }
                    if (!db.VehicleRequestTypes.Any())
                    {
                        db.VehicleRequestTypes.AddRange(
                    new VehicleRequestType { Id = 3, Name = "Talep Türü 1", Description = "Lorem ipsum dolor sit amet" },
                    new VehicleRequestType { Id = 4, Name = "Talep Türü 2", Description = "Consectetur adipiscing elit" }
                        );
                    }
                    if (!db.Vehicles.Any())
                    {
                        db.Vehicles.AddRange(
                    new Vehicle { Id = 5, Plate = "Sample Item 1", Brand = "Sample Item 1", Model = "Sample Item 1", Year = 42, Capacity = 42, Status = (VehicleStatus)0 },
                    new Vehicle { Id = 6, Plate = "Sample Item 2", Brand = "Sample Item 2", Model = "Sample Item 2", Year = 17, Capacity = 17, Status = (VehicleStatus)1 }
                        );
                    }
                    if (!db.VehicleRequests.Any())
                    {
                        db.VehicleRequests.AddRange(
                    new VehicleRequest { Id = 7, RequestNo = "ABC-001", RequestTypeId = 1000L, StartDate = new DateTime(2024, 3, 15), EndDate = new DateTime(2024, 3, 15), Destination = "Sample Item 1", Purpose = "Sample Item 1", DriverName = "Araç Talebi 1", IsPoolExternal = true, ExternalVehicleInfo = "Sample Item 1", FleetNote = "Lorem ipsum dolor sit amet", RevisionNote = "Lorem ipsum dolor sit amet", Status = (VehicleRequestStatus)0, DepartmentId = 1, VehicleRequestTypeId = 3, VehicleId = 5 },
                    new VehicleRequest { Id = 8, RequestNo = "XYZ-002", RequestTypeId = 2000L, StartDate = new DateTime(2024, 6, 20), EndDate = new DateTime(2024, 6, 20), Destination = "Sample Item 2", Purpose = "Sample Item 2", DriverName = "Araç Talebi 2", IsPoolExternal = false, ExternalVehicleInfo = "Sample Item 2", FleetNote = "Consectetur adipiscing elit", RevisionNote = "Consectetur adipiscing elit", Status = (VehicleRequestStatus)0, DepartmentId = 2, VehicleRequestTypeId = 4, VehicleId = 6 }
                        );
                    }
                            db.SaveChanges();
                            Console.WriteLine("[Seed] Sample data created.");
                        }
                        catch (Exception sampleEx)
                        {
                            Console.WriteLine($"[Seed] Sample data skipped: {sampleEx.GetType().Name}: {sampleEx.Message}");
                            // Carry on — RBAC seed must still run so admin/123qwe is usable.
                        }
                        // Sync identity sequences to MAX(Id). Seeded rows carry explicit Ids which do NOT
                        // advance Postgres identity sequences → nextval collides with a seed row and the
                        // first few inserts fail with a duplicate-key 500. Runs every startup; idempotent.
                        try
                        {
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Departments\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Departments\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"VehicleRequestTypes\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"VehicleRequestTypes\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Vehicles\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Vehicles\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"VehicleRequests\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"VehicleRequests\") + 1, false);");
                            Console.WriteLine("[Seed] Identity sequences synced.");
                        }
                        catch (Exception seqEx)
                        {
                            Console.WriteLine($"[Seed] Sequence sync skipped: {seqEx.GetType().Name}: {seqEx.Message}");
                        }
                    }
                    // RBAC seed (Admin/User roles + permissions + admin user) runs through ABP DI
                    // so PermissionRegistry can be injected. SeedHelper is idempotent.
                    SeedHelper.SeedHostDb(Abp.Dependency.IocManager.Instance);
                    Console.WriteLine("[Seed] RBAC seed complete (Admin role + admin user).");
                }
                catch (Exception ex)
                {
                    // Full diagnostic — surface the real cause so silent seed failures are debuggable.
                    Console.WriteLine($"[Migration] FAILED: {ex.GetType().Name}: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"[Migration] InnerException: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                    Console.WriteLine("[Migration] StackTrace:");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("[Migration] App continues without migration — admin user will not exist.");
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public class Program
    {
        // Runtime entry: WebHost is required because ABP Startup returns IServiceProvider.
        public static void Main(string[] args)
        {
            // Npgsql 7+ requires UTC DateTimes — enable legacy behavior for ABP compatibility
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        // Design-time entry for EF Core tools (dotnet ef migrations).
        // Without this, EF tools wait 5 minutes for IHost build (resolver default timeout)
        // and then SIGTERM any running dotnet process — killing live dev servers.
        // We expose a minimal IHost that EF tools resolve in milliseconds; the actual
        // DbContext is built by IDesignTimeDbContextFactory in the EntityFrameworkCore project.
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);
    }
}
