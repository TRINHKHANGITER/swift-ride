using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trip_Service.Migrations
{
    /// <inheritdoc />
    public partial class AddTripDriverLocationProjection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DriverLatitude",
                table: "Trips",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DriverLocationUpdatedAt",
                table: "Trips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DriverLongitude",
                table: "Trips",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLatitude",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DriverLocationUpdatedAt",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DriverLongitude",
                table: "Trips");
        }
    }
}
