using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddReturnandReturnItemandScratchandScratchItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Return",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    OutletId = table.Column<int>(nullable: false),
                    WarehouseId = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Return", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Return_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Return_Outlet_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Return_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Scratch",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    WarehouseId = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scratch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scratch_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Scratch_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReturnITem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    BatchId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    QtyInSmallestUnit = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnITem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnITem_Batch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnITem_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnITem_Return_ReturnId",
                        column: x => x.ReturnId,
                        principalTable: "Return",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnITem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScratchItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScratchId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false),
                    BatchId = table.Column<int>(nullable: false),
                    UnitName = table.Column<string>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    QtyInSmallestUnit = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScratchItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScratchItem_Batch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScratchItem_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScratchItem_Scratch_ScratchId",
                        column: x => x.ScratchId,
                        principalTable: "Scratch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScratchItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Return_BranchId",
                table: "Return",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Return_OutletId",
                table: "Return",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Return_WarehouseId",
                table: "Return",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnITem_BatchId",
                table: "ReturnITem",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnITem_ItemId",
                table: "ReturnITem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnITem_ReturnId",
                table: "ReturnITem",
                column: "ReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnITem_UnitId",
                table: "ReturnITem",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Scratch_BranchId",
                table: "Scratch",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Scratch_WarehouseId",
                table: "Scratch",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ScratchItem_BatchId",
                table: "ScratchItem",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ScratchItem_ItemId",
                table: "ScratchItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ScratchItem_ScratchId",
                table: "ScratchItem",
                column: "ScratchId");

            migrationBuilder.CreateIndex(
                name: "IX_ScratchItem_UnitId",
                table: "ScratchItem",
                column: "UnitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReturnITem");

            migrationBuilder.DropTable(
                name: "ScratchItem");

            migrationBuilder.DropTable(
                name: "Return");

            migrationBuilder.DropTable(
                name: "Scratch");
        }
    }
}
