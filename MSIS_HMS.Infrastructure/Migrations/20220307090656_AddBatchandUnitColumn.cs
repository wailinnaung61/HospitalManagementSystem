using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddBatchandUnitColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BatchId",
                table: "GroundingItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "GroundingItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GroundingItem_BatchId",
                table: "GroundingItem",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_GroundingItem_UnitId",
                table: "GroundingItem",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroundingItem_Batch_BatchId",
                table: "GroundingItem",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroundingItem_Unit_UnitId",
                table: "GroundingItem",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroundingItem_Batch_BatchId",
                table: "GroundingItem");

            migrationBuilder.DropForeignKey(
                name: "FK_GroundingItem_Unit_UnitId",
                table: "GroundingItem");

            migrationBuilder.DropIndex(
                name: "IX_GroundingItem_BatchId",
                table: "GroundingItem");

            migrationBuilder.DropIndex(
                name: "IX_GroundingItem_UnitId",
                table: "GroundingItem");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "GroundingItem");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "GroundingItem");
        }
    }
}
