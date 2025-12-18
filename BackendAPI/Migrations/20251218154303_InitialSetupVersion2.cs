using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetupVersion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinishedGoodsInventories_Users_UpdatedByUserUserId",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_CreatedByUserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UpdatedByUserUserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterialInventories_Users_CreatedByUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterialInventories_Users_UpdatedByUserUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Users_UpdatedByUserUserId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_UpdatedByUserUserId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_RawMaterialInventories_UpdatedByUserUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropIndex(
                name: "IX_Products_UpdatedByUserUserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_FinishedGoodsInventories_UpdatedByUserUserId",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserUserId",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserUserId",
                table: "FinishedGoodsInventories");

            migrationBuilder.CreateIndex(
                name: "IX_RawMaterials_CreatedByUserId",
                table: "RawMaterials",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FinishedGoodsInventories_CreatedByUserId",
                table: "FinishedGoodsInventories",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinishedGoodsInventories_Users_CreatedByUserId",
                table: "FinishedGoodsInventories",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_CreatedByUserId",
                table: "Products",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterialInventories_Users_CreatedByUserId",
                table: "RawMaterialInventories",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterials_Users_CreatedByUserId",
                table: "RawMaterials",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinishedGoodsInventories_Users_CreatedByUserId",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_CreatedByUserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterialInventories_Users_CreatedByUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterials_Users_CreatedByUserId",
                table: "RawMaterials");

            migrationBuilder.DropIndex(
                name: "IX_RawMaterials_CreatedByUserId",
                table: "RawMaterials");

            migrationBuilder.DropIndex(
                name: "IX_FinishedGoodsInventories_CreatedByUserId",
                table: "FinishedGoodsInventories");

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserUserId",
                table: "SalesOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserUserId",
                table: "RawMaterialInventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserUserId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserUserId",
                table: "FinishedGoodsInventories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_UpdatedByUserUserId",
                table: "SalesOrders",
                column: "UpdatedByUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RawMaterialInventories_UpdatedByUserUserId",
                table: "RawMaterialInventories",
                column: "UpdatedByUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UpdatedByUserUserId",
                table: "Products",
                column: "UpdatedByUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FinishedGoodsInventories_UpdatedByUserUserId",
                table: "FinishedGoodsInventories",
                column: "UpdatedByUserUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinishedGoodsInventories_Users_UpdatedByUserUserId",
                table: "FinishedGoodsInventories",
                column: "UpdatedByUserUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_CreatedByUserId",
                table: "Products",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_UpdatedByUserUserId",
                table: "Products",
                column: "UpdatedByUserUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterialInventories_Users_CreatedByUserId",
                table: "RawMaterialInventories",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterialInventories_Users_UpdatedByUserUserId",
                table: "RawMaterialInventories",
                column: "UpdatedByUserUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Users_UpdatedByUserUserId",
                table: "SalesOrders",
                column: "UpdatedByUserUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
