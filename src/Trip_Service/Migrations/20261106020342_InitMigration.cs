using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trip_Service.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Uuid);
                });

            migrationBuilder.CreateTable(
                name: "DriverProfiles",
                columns: table => new
                {
                    DriverUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverProfiles", x => x.DriverUuid);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiderUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PickupLatitude = table.Column<double>(type: "float", nullable: false),
                    PickupLongitude = table.Column<double>(type: "float", nullable: false),
                    PickupName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DestinationLatitude = table.Column<double>(type: "float", nullable: false),
                    DestinationLongitude = table.Column<double>(type: "float", nullable: false),
                    DestinationName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DistanceKM = table.Column<double>(type: "float", nullable: true),
                    DurationMin = table.Column<double>(type: "float", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Uuid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "DriverProfiles");

            migrationBuilder.DropTable(
                name: "Trips");
        }
    }
}
