using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixFinishedGoodsRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "FinishedGoodsInventories",
                newName: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_FinishedGoodsInventories_ProductID",
                table: "FinishedGoodsInventories",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_FinishedGoodsInventories_Products_ProductID",
                table: "FinishedGoodsInventories",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinishedGoodsInventories_Products_ProductID",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropIndex(
                name: "IX_FinishedGoodsInventories_ProductID",
                table: "FinishedGoodsInventories");

            migrationBuilder.RenameColumn(
                name: "ProductID",
                table: "FinishedGoodsInventories",
                newName: "ProductId");
        }
    }
}
