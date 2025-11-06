using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddDeliverOrderAndDeliverOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliverOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherNo = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Customer = table.Column<string>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Discount = table.Column<double>(nullable: false),
                    Tax = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    BranchId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliverOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliverOrder_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliverOrder_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliverOrderItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliverOrderId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false),
                    BatchId = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    UnitName = table.Column<string>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    QtyInSmallestUnit = table.Column<int>(nullable: false),
                    IsGiftItem = table.Column<bool>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliverOrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliverOrderItem_Batch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliverOrderItem_DeliverOrder_DeliverOrderId",
                        column: x => x.DeliverOrderId,
                        principalTable: "DeliverOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliverOrderItem_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliverOrderItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliverOrder_BranchId",
                table: "DeliverOrder",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliverOrder_WarehouseId",
                table: "DeliverOrder",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliverOrderItem_BatchId",
                table: "DeliverOrderItem",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliverOrderItem_DeliverOrderId",
                table: "DeliverOrderItem",
                column: "DeliverOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliverOrderItem_ItemId",
                table: "DeliverOrderItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliverOrderItem_UnitId",
                table: "DeliverOrderItem",
                column: "UnitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliverOrderItem");

            migrationBuilder.DropTable(
                name: "DeliverOrder");
        }
    }
}
