using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddGroundingandGroundingItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grounding",
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
                    WarehouseId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grounding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grounding_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroundingItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroundingId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    PreviouseQty = table.Column<int>(nullable: false),
                    ChangedQty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroundingItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroundingItem_Grounding_GroundingId",
                        column: x => x.GroundingId,
                        principalTable: "Grounding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroundingItem_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grounding_BranchId",
                table: "Grounding",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_GroundingItem_GroundingId",
                table: "GroundingItem",
                column: "GroundingId");

            migrationBuilder.CreateIndex(
                name: "IX_GroundingItem_ItemId",
                table: "GroundingItem",
                column: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroundingItem");

            migrationBuilder.DropTable(
                name: "Grounding");
        }
    }
}
