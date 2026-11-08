using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment_Service.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransferCode",
                table: "Payments",
                column: "TransferCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TripUuid",
                table: "Payments",
                column: "TripUuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_TransferCode",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TripUuid",
                table: "Payments");
        }
    }
}
