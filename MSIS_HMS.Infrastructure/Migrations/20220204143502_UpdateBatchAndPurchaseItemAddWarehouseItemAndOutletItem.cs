using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class UpdateBatchAndPurchaseItemAddWarehouseItemAndOutletItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseItems",
                table: "PurchaseItems");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Batch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseItems",
                table: "PurchaseItems",
                columns: new[] { "PurchaseId", "ItemId", "UnitId" });

            migrationBuilder.CreateTable(
                name: "OutletItem",
                columns: table => new
                {
                    OutletId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    Qty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletItem", x => new { x.OutletId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_OutletItem_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletItem_Outlet_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseItem",
                columns: table => new
                {
                    WarehouseId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    BatchId = table.Column<int>(nullable: false),
                    Qty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseItem", x => new { x.WarehouseId, x.ItemId, x.BatchId });
                    table.ForeignKey(
                        name: "FK_WarehouseItem_Batch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseItem_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseItem_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Batch_BranchId",
                table: "Batch",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletItem_ItemId",
                table: "OutletItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseItem_BatchId",
                table: "WarehouseItem",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseItem_ItemId",
                table: "WarehouseItem",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batch_Branch_BranchId",
                table: "Batch",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batch_Branch_BranchId",
                table: "Batch");

            migrationBuilder.DropTable(
                name: "OutletItem");

            migrationBuilder.DropTable(
                name: "WarehouseItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseItems",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_Batch_BranchId",
                table: "Batch");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Batch");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseItems",
                table: "PurchaseItems",
                columns: new[] { "PurchaseId", "ItemId" });
        }
    }
}
