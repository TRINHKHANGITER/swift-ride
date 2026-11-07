using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trip_Service.Migrations
{
    /// <inheritdoc />
    public partial class AddTripDriverRejections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TripDriverRejections",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripDriverRejections", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_TripDriverRejections_DriverProfiles_DriverUuid",
                        column: x => x.DriverUuid,
                        principalTable: "DriverProfiles",
                        principalColumn: "DriverUuid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TripDriverRejections_Trips_TripUuid",
                        column: x => x.TripUuid,
                        principalTable: "Trips",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripDriverRejections_DriverUuid",
                table: "TripDriverRejections",
                column: "DriverUuid");

            migrationBuilder.CreateIndex(
                name: "IX_TripDriverRejections_TripUuid_DriverUuid",
                table: "TripDriverRejections",
                columns: new[] { "TripUuid", "DriverUuid" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripDriverRejections");
        }
    }
}
