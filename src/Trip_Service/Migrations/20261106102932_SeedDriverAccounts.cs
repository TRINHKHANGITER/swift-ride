using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Trip_Service.Migrations
{
    /// <inheritdoc />
    public partial class SeedDriverAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Uuid", "Fullname", "Password", "Phone", "Role" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Tài xế 01", "AQAAAAIAAYagAAAAEGccaZPJAH4KF00h5YrSUBkfiHEfQzVYYAfwJ2m6dnoWQ9rO1sdKMvlX2XMgFdN29Q==", "0900000001", 1 },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Tài xế 02", "AQAAAAIAAYagAAAAEGccaZPJAH4KF00h5YrSUBkfiHEfQzVYYAfwJ2m6dnoWQ9rO1sdKMvlX2XMgFdN29Q==", "0900000002", 1 },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Tài xế 03", "AQAAAAIAAYagAAAAEGccaZPJAH4KF00h5YrSUBkfiHEfQzVYYAfwJ2m6dnoWQ9rO1sdKMvlX2XMgFdN29Q==", "0900000003", 1 },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Tài xế 04", "AQAAAAIAAYagAAAAEGccaZPJAH4KF00h5YrSUBkfiHEfQzVYYAfwJ2m6dnoWQ9rO1sdKMvlX2XMgFdN29Q==", "0900000004", 1 },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "Tài xế 05", "AQAAAAIAAYagAAAAEGccaZPJAH4KF00h5YrSUBkfiHEfQzVYYAfwJ2m6dnoWQ9rO1sdKMvlX2XMgFdN29Q==", "0900000005", 1 },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "Tài xế 06", "AQAAAAIAAYagAAAAEGccaZPJAH4KF00h5YrSUBkfiHEfQzVYYAfwJ2m6dnoWQ9rO1sdKMvlX2XMgFdN29Q==", "0900000006", 1 }
                });

            migrationBuilder.InsertData(
                table: "DriverProfiles",
                columns: new[] { "DriverUuid", "AccountUuid", "IsVerified", "LicenseNumber", "LicensePlate", "VehicleName" },
                values: new object[,]
                {
                    { new Guid("1e6b9c74-5d32-48af-b103-7c2e9a6f4d81"), new Guid("10000000-0000-0000-0000-000000000003"), true, "GPLX000003", "51A-100.03", "Yamaha Sirius" },
                    { new Guid("4d7a2c98-e153-46b0-97af-8c5e1d3b6294"), new Guid("10000000-0000-0000-0000-000000000006"), true, "GPLX000006", "51A-100.06", "Honda Future" },
                    { new Guid("7f9c2b1e-4a63-4d85-9b27-6e1f3c8a5d42"), new Guid("10000000-0000-0000-0000-000000000001"), true, "GPLX000001", "51A-100.01", "Honda Wave Alpha" },
                    { new Guid("a52d7e19-c684-4f3b-8a20-9d1c5e7b436f"), new Guid("10000000-0000-0000-0000-000000000004"), true, "GPLX000004", "51A-100.04", "Honda Air Blade" },
                    { new Guid("c3a8f6d2-91b4-47e0-a56c-2d9f8b7e1345"), new Guid("10000000-0000-0000-0000-000000000002"), true, "GPLX000002", "51A-100.02", "Honda Vision" },
                    { new Guid("e8b41d63-2f95-4ac7-b682-3e0d9f1a754c"), new Guid("10000000-0000-0000-0000-000000000005"), true, "GPLX000005", "51A-100.05", "Yamaha Janus" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Uuid",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Uuid",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Uuid",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Uuid",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Uuid",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Uuid",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "DriverProfiles",
                keyColumn: "DriverUuid",
                keyValue: new Guid("1e6b9c74-5d32-48af-b103-7c2e9a6f4d81"));

            migrationBuilder.DeleteData(
                table: "DriverProfiles",
                keyColumn: "DriverUuid",
                keyValue: new Guid("4d7a2c98-e153-46b0-97af-8c5e1d3b6294"));

            migrationBuilder.DeleteData(
                table: "DriverProfiles",
                keyColumn: "DriverUuid",
                keyValue: new Guid("7f9c2b1e-4a63-4d85-9b27-6e1f3c8a5d42"));

            migrationBuilder.DeleteData(
                table: "DriverProfiles",
                keyColumn: "DriverUuid",
                keyValue: new Guid("a52d7e19-c684-4f3b-8a20-9d1c5e7b436f"));

            migrationBuilder.DeleteData(
                table: "DriverProfiles",
                keyColumn: "DriverUuid",
                keyValue: new Guid("c3a8f6d2-91b4-47e0-a56c-2d9f8b7e1345"));

            migrationBuilder.DeleteData(
                table: "DriverProfiles",
                keyColumn: "DriverUuid",
                keyValue: new Guid("e8b41d63-2f95-4ac7-b682-3e0d9f1a754c"));
        }
    }
}
