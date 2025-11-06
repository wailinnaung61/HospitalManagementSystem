using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddBranchIdToLocationAndItemLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Location",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "ItemLocation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Location_BranchId",
                table: "Location",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLocation_BranchId",
                table: "ItemLocation",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLocation_Branch_BranchId",
                table: "ItemLocation",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Branch_BranchId",
                table: "Location",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemLocation_Branch_BranchId",
                table: "ItemLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_Location_Branch_BranchId",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_BranchId",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_ItemLocation_BranchId",
                table: "ItemLocation");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ItemLocation");
        }
    }
}
