using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBomUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BOMs_ProductId_RawMaterialId",
                table: "BOMs");

            migrationBuilder.CreateIndex(
                name: "IX_BOMs_ProductId_RawMaterialId",
                table: "BOMs",
                columns: new[] { "ProductId", "RawMaterialId" },
                unique: true,
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BOMs_ProductId_RawMaterialId",
                table: "BOMs");

            migrationBuilder.CreateIndex(
                name: "IX_BOMs_ProductId_RawMaterialId",
                table: "BOMs",
                columns: new[] { "ProductId", "RawMaterialId" },
                unique: true);
        }
    }
}
