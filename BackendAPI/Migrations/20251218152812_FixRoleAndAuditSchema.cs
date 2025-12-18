using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixRoleAndAuditSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BOM_Products_ProductId",
                table: "BOM");

            migrationBuilder.DropForeignKey(
                name: "FK_BOM_RawMaterials_RawMaterialId",
                table: "BOM");

            migrationBuilder.DropForeignKey(
                name: "FK_BOM_Users_CreatedByUserId",
                table: "BOM");

            migrationBuilder.DropForeignKey(
                name: "FK_FinishedGoodsInventories_Users_UpdatedByUserId",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterialInventories_Users_UpdatedByUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BOM",
                table: "BOM");

            migrationBuilder.RenameTable(
                name: "BOM",
                newName: "BOMs");

            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "RawMaterialInventories",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "FinishedGoodsInventories",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_BOM_RawMaterialId",
                table: "BOMs",
                newName: "IX_BOMs_RawMaterialId");

            migrationBuilder.RenameIndex(
                name: "IX_BOM_ProductId_RawMaterialId",
                table: "BOMs",
                newName: "IX_BOMs_ProductId_RawMaterialId");

            migrationBuilder.RenameIndex(
                name: "IX_BOM_CreatedByUserId",
                table: "BOMs",
                newName: "IX_BOMs_CreatedByUserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SalesOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SalesOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SalesOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "SalesOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserUserId",
                table: "SalesOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RawMaterials",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "RawMaterials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RawMaterials",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RawMaterials",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "RawMaterials",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedByUserId",
                table: "RawMaterialInventories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "RawMaterialInventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RawMaterialInventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RawMaterialInventories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserUserId",
                table: "RawMaterialInventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserUserId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductionOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProductionOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductionOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "ProductionOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedByUserId",
                table: "FinishedGoodsInventories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "FinishedGoodsInventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FinishedGoodsInventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "FinishedGoodsInventories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserUserId",
                table: "FinishedGoodsInventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BOMs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BOMs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "BOMs",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BOMs",
                table: "BOMs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_UpdatedByUserId",
                table: "SalesOrders",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_UpdatedByUserUserId",
                table: "SalesOrders",
                column: "UpdatedByUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RawMaterials_UpdatedByUserId",
                table: "RawMaterials",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RawMaterialInventories_CreatedByUserId",
                table: "RawMaterialInventories",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RawMaterialInventories_UpdatedByUserUserId",
                table: "RawMaterialInventories",
                column: "UpdatedByUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UpdatedByUserId",
                table: "Products",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UpdatedByUserUserId",
                table: "Products",
                column: "UpdatedByUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrders_UpdatedByUserId",
                table: "ProductionOrders",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FinishedGoodsInventories_UpdatedByUserUserId",
                table: "FinishedGoodsInventories",
                column: "UpdatedByUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BOMs_UpdatedByUserId",
                table: "BOMs",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BOMs_Products_ProductId",
                table: "BOMs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BOMs_RawMaterials_RawMaterialId",
                table: "BOMs",
                column: "RawMaterialId",
                principalTable: "RawMaterials",
                principalColumn: "RawMaterialId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BOMs_Users_CreatedByUserId",
                table: "BOMs",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BOMs_Users_UpdatedByUserId",
                table: "BOMs",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinishedGoodsInventories_Users_UpdatedByUserId",
                table: "FinishedGoodsInventories",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinishedGoodsInventories_Users_UpdatedByUserUserId",
                table: "FinishedGoodsInventories",
                column: "UpdatedByUserUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionOrders_Users_UpdatedByUserId",
                table: "ProductionOrders",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_UpdatedByUserId",
                table: "Products",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_RawMaterialInventories_Users_UpdatedByUserId",
                table: "RawMaterialInventories",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterialInventories_Users_UpdatedByUserUserId",
                table: "RawMaterialInventories",
                column: "UpdatedByUserUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterials_Users_UpdatedByUserId",
                table: "RawMaterials",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Users_UpdatedByUserId",
                table: "SalesOrders",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrders_Users_UpdatedByUserUserId",
                table: "SalesOrders",
                column: "UpdatedByUserUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BOMs_Products_ProductId",
                table: "BOMs");

            migrationBuilder.DropForeignKey(
                name: "FK_BOMs_RawMaterials_RawMaterialId",
                table: "BOMs");

            migrationBuilder.DropForeignKey(
                name: "FK_BOMs_Users_CreatedByUserId",
                table: "BOMs");

            migrationBuilder.DropForeignKey(
                name: "FK_BOMs_Users_UpdatedByUserId",
                table: "BOMs");

            migrationBuilder.DropForeignKey(
                name: "FK_FinishedGoodsInventories_Users_UpdatedByUserId",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_FinishedGoodsInventories_Users_UpdatedByUserUserId",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionOrders_Users_UpdatedByUserId",
                table: "ProductionOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UpdatedByUserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UpdatedByUserUserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterialInventories_Users_CreatedByUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterialInventories_Users_UpdatedByUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterialInventories_Users_UpdatedByUserUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterials_Users_UpdatedByUserId",
                table: "RawMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Users_UpdatedByUserId",
                table: "SalesOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrders_Users_UpdatedByUserUserId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_UpdatedByUserId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrders_UpdatedByUserUserId",
                table: "SalesOrders");

            migrationBuilder.DropIndex(
                name: "IX_RawMaterials_UpdatedByUserId",
                table: "RawMaterials");

            migrationBuilder.DropIndex(
                name: "IX_RawMaterialInventories_CreatedByUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropIndex(
                name: "IX_RawMaterialInventories_UpdatedByUserUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropIndex(
                name: "IX_Products_UpdatedByUserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UpdatedByUserUserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductionOrders_UpdatedByUserId",
                table: "ProductionOrders");

            migrationBuilder.DropIndex(
                name: "IX_FinishedGoodsInventories_UpdatedByUserUserId",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BOMs",
                table: "BOMs");

            migrationBuilder.DropIndex(
                name: "IX_BOMs_UpdatedByUserId",
                table: "BOMs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserUserId",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RawMaterialInventories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RawMaterialInventories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserUserId",
                table: "RawMaterialInventories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserUserId",
                table: "FinishedGoodsInventories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BOMs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BOMs");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "BOMs");

            migrationBuilder.RenameTable(
                name: "BOMs",
                newName: "BOM");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "RawMaterialInventories",
                newName: "LastUpdated");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "FinishedGoodsInventories",
                newName: "LastUpdated");

            migrationBuilder.RenameIndex(
                name: "IX_BOMs_RawMaterialId",
                table: "BOM",
                newName: "IX_BOM_RawMaterialId");

            migrationBuilder.RenameIndex(
                name: "IX_BOMs_ProductId_RawMaterialId",
                table: "BOM",
                newName: "IX_BOM_ProductId_RawMaterialId");

            migrationBuilder.RenameIndex(
                name: "IX_BOMs_CreatedByUserId",
                table: "BOM",
                newName: "IX_BOM_CreatedByUserId");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedByUserId",
                table: "RawMaterialInventories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedByUserId",
                table: "FinishedGoodsInventories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BOM",
                table: "BOM",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BOM_Products_ProductId",
                table: "BOM",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BOM_RawMaterials_RawMaterialId",
                table: "BOM",
                column: "RawMaterialId",
                principalTable: "RawMaterials",
                principalColumn: "RawMaterialId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BOM_Users_CreatedByUserId",
                table: "BOM",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinishedGoodsInventories_Users_UpdatedByUserId",
                table: "FinishedGoodsInventories",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterialInventories_Users_UpdatedByUserId",
                table: "RawMaterialInventories",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
