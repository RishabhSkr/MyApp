using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPlannedDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "ProductionOrders",
                newName: "PlannedStartDate");

            migrationBuilder.RenameColumn(
                name: "CompletedDate",
                table: "ProductionOrders",
                newName: "PlannedEndDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlannedStartDate",
                table: "ProductionOrders",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "PlannedEndDate",
                table: "ProductionOrders",
                newName: "CompletedDate");
        }
    }
}
