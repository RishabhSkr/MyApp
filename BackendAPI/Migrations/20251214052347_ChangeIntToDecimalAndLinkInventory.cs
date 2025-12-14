using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIntToDecimalAndLinkInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RawMaterialInventories_RawMaterialID",
                table: "RawMaterialInventories");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "SalesOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "ProducedQuantity",
                table: "ProductionOrders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "PlannedQuantity",
                table: "ProductionOrders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "ProductionOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "AvailableQuantity",
                table: "FinishedGoodsInventories",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CreatedByUserId",
                table: "SalesOrders",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RawMaterialInventories_RawMaterialID",
                table: "RawMaterialInventories",
                column: "RawMaterialID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrders_CreatedByUserId",
                table: "ProductionOrders",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionOrders_Users_CreatedByUserId",
                table: "ProductionOrders",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Users_CreatedByUserId",
                table: "SalesOrders",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionOrders_Users_CreatedByUserId",
                table: "ProductionOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Users_CreatedByUserId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_CreatedByUserId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_RawMaterialInventories_RawMaterialID",
                table: "RawMaterialInventories");

            migrationBuilder.DropIndex(
                name: "IX_ProductionOrders_CreatedByUserId",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ProductionOrders");

            migrationBuilder.AlterColumn<int>(
                name: "ProducedQuantity",
                table: "ProductionOrders",
                type: "int",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "PlannedQuantity",
                table: "ProductionOrders",
                type: "int",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "AvailableQuantity",
                table: "FinishedGoodsInventories",
                type: "int",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.CreateIndex(
                name: "IX_RawMaterialInventories_RawMaterialID",
                table: "RawMaterialInventories",
                column: "RawMaterialID");
        }
    }
}
