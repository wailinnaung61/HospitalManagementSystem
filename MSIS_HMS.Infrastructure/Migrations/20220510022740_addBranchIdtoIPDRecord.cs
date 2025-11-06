using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class addBranchIdtoIPDRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "IPDRecord",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IPDRecord_BranchId",
                table: "IPDRecord",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_IPDRecord_Branch_BranchId",
                table: "IPDRecord",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPDRecord_Branch_BranchId",
                table: "IPDRecord");

            migrationBuilder.DropIndex(
                name: "IX_IPDRecord_BranchId",
                table: "IPDRecord");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "IPDRecord");
        }
    }
}
