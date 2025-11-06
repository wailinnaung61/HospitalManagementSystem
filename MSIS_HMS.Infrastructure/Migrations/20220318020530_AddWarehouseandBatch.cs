using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddWarehouseandBatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BatchId",
                table: "ItemLocation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "ItemLocation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ItemLocation_BatchId",
                table: "ItemLocation",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLocation_WarehouseId",
                table: "ItemLocation",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLocation_Batch_BatchId",
                table: "ItemLocation",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLocation_Warehouse_WarehouseId",
                table: "ItemLocation",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemLocation_Batch_BatchId",
                table: "ItemLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemLocation_Warehouse_WarehouseId",
                table: "ItemLocation");

            migrationBuilder.DropIndex(
                name: "IX_ItemLocation_BatchId",
                table: "ItemLocation");

            migrationBuilder.DropIndex(
                name: "IX_ItemLocation_WarehouseId",
                table: "ItemLocation");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "ItemLocation");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "ItemLocation");
        }
    }
}
