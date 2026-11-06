using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trip_Service.Migrations
{
    /// <inheritdoc />
    public partial class AddTripRouteAndDriverLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "RouteGeometryJson",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLatitude",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DriverLongitude",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "RouteGeometryJson",
                table: "Trips");
        }
    }
}
