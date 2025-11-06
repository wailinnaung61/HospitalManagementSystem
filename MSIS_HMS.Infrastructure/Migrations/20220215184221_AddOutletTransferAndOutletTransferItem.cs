using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddOutletTransferAndOutletTransferItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutletTransfer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    WarehouseId = table.Column<int>(nullable: false),
                    OutletId = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_OutletTransfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutletTransfer_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletTransfer_Outlet_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletTransfer_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutletTransferItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutletTransferId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    BatchId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    QtyInSmallestUnit = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletTransferItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutletTransferItem_Batch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletTransferItem_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletTransferItem_OutletTransfer_OutletTransferId",
                        column: x => x.OutletTransferId,
                        principalTable: "OutletTransfer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletTransferItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutletTransfer_BranchId",
                table: "OutletTransfer",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTransfer_OutletId",
                table: "OutletTransfer",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTransfer_WarehouseId",
                table: "OutletTransfer",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTransferItem_BatchId",
                table: "OutletTransferItem",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTransferItem_ItemId",
                table: "OutletTransferItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTransferItem_OutletTransferId",
                table: "OutletTransferItem",
                column: "OutletTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletTransferItem_UnitId",
                table: "OutletTransferItem",
                column: "UnitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutletTransferItem");

            migrationBuilder.DropTable(
                name: "OutletTransfer");
        }
    }
}
