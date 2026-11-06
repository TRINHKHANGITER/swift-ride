using Microsoft.EntityFrameworkCore;
using Trip_Service.Domain;

namespace Trip_Service.Infrastructure.Persistence;

public class TripDbContext : DbContext
{
    public TripDbContext(DbContextOptions<TripDbContext> options) : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<DriverProfile> DriverProfiles => Set<DriverProfile>();
    public DbSet<Trip> Trips => Set<Trip>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Driver@123
        const string driverPasswordHash = "AQAAAAIAAYagAAAAEGccaZPJAH4KF00h5YrSUBkfiHEfQzVYYAfwJ2m6dnoWQ9rO1sdKMvlX2XMgFdN29Q==";

        var driverAccounts = new[]
        {
            new Account { Uuid = new Guid("10000000-0000-0000-0000-000000000001"), Fullname = "Tài xế 01", Phone = "0900000001", Password = driverPasswordHash, Role = Role.Driver },
            new Account { Uuid = new Guid("10000000-0000-0000-0000-000000000002"), Fullname = "Tài xế 02", Phone = "0900000002", Password = driverPasswordHash, Role = Role.Driver },
            new Account { Uuid = new Guid("10000000-0000-0000-0000-000000000003"), Fullname = "Tài xế 03", Phone = "0900000003", Password = driverPasswordHash, Role = Role.Driver },
            new Account { Uuid = new Guid("10000000-0000-0000-0000-000000000004"), Fullname = "Tài xế 04", Phone = "0900000004", Password = driverPasswordHash, Role = Role.Driver },
            new Account { Uuid = new Guid("10000000-0000-0000-0000-000000000005"), Fullname = "Tài xế 05", Phone = "0900000005", Password = driverPasswordHash, Role = Role.Driver },
            new Account { Uuid = new Guid("10000000-0000-0000-0000-000000000006"), Fullname = "Tài xế 06", Phone = "0900000006", Password = driverPasswordHash, Role = Role.Driver }
        };

        var driverProfiles = new[]
        {
            new DriverProfile { DriverUuid = new Guid("7f9c2b1e-4a63-4d85-9b27-6e1f3c8a5d42"), AccountUuid = driverAccounts[0].Uuid, LicenseNumber = "GPLX000001", VehicleName = "Honda Wave Alpha", LicensePlate = "51A-100.01", IsVerified = true },
            new DriverProfile { DriverUuid = new Guid("c3a8f6d2-91b4-47e0-a56c-2d9f8b7e1345"), AccountUuid = driverAccounts[1].Uuid, LicenseNumber = "GPLX000002", VehicleName = "Honda Vision", LicensePlate = "51A-100.02", IsVerified = true },
            new DriverProfile { DriverUuid = new Guid("1e6b9c74-5d32-48af-b103-7c2e9a6f4d81"), AccountUuid = driverAccounts[2].Uuid, LicenseNumber = "GPLX000003", VehicleName = "Yamaha Sirius", LicensePlate = "51A-100.03", IsVerified = true },
            new DriverProfile { DriverUuid = new Guid("a52d7e19-c684-4f3b-8a20-9d1c5e7b436f"), AccountUuid = driverAccounts[3].Uuid, LicenseNumber = "GPLX000004", VehicleName = "Honda Air Blade", LicensePlate = "51A-100.04", IsVerified = true },
            new DriverProfile { DriverUuid = new Guid("e8b41d63-2f95-4ac7-b682-3e0d9f1a754c"), AccountUuid = driverAccounts[4].Uuid, LicenseNumber = "GPLX000005", VehicleName = "Yamaha Janus", LicensePlate = "51A-100.05", IsVerified = true },
            new DriverProfile { DriverUuid = new Guid("4d7a2c98-e153-46b0-97af-8c5e1d3b6294"), AccountUuid = driverAccounts[5].Uuid, LicenseNumber = "GPLX000006", VehicleName = "Honda Future", LicensePlate = "51A-100.06", IsVerified = true }
        };

        modelBuilder.Entity<Account>().HasData(driverAccounts);
        modelBuilder.Entity<DriverProfile>().HasData(driverProfiles);

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.Property(trip => trip.DistanceKM).HasPrecision(10, 2);
            entity.Property(trip => trip.DurationMin).HasPrecision(10, 2);
            entity.Property(trip => trip.Price).HasPrecision(18, 0);
        });
    }
}
