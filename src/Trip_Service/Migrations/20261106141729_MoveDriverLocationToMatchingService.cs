using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trip_Service.Migrations
{
    /// <inheritdoc />
    public partial class MoveDriverLocationToMatchingService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLatitude",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DriverLongitude",
                table: "Trips");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DriverLatitude",
                table: "Trips",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DriverLongitude",
                table: "Trips",
                type: "float",
                nullable: true);
        }
    }
}
