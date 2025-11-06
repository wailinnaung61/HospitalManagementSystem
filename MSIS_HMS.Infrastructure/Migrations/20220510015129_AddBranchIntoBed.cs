using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddBranchIntoBed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Room",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Bed",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Room_BranchId",
                table: "Room",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Bed_BranchId",
                table: "Bed",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bed_Branch_BranchId",
                table: "Bed",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Branch_BranchId",
                table: "Room",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bed_Branch_BranchId",
                table: "Bed");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_Branch_BranchId",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Room_BranchId",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Bed_BranchId",
                table: "Bed");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Bed");

            migrationBuilder.AddColumn<int>(
                name: "TreatmentProcess",
                table: "IPDRecord",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
