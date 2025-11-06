using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class RemoveBranchFromOTDoctorandStaff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OT_Doctor_Branch_BranchId",
                table: "OT_Doctor");

            migrationBuilder.DropForeignKey(
                name: "FK_OT_Staff_Branch_BranchId",
                table: "OT_Staff");

            migrationBuilder.DropIndex(
                name: "IX_OT_Staff_BranchId",
                table: "OT_Staff");

            migrationBuilder.DropIndex(
                name: "IX_OT_Doctor_BranchId",
                table: "OT_Doctor");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "OT_Staff");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "OT_Doctor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "OT_Staff",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "OT_Doctor",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OT_Staff_BranchId",
                table: "OT_Staff",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OT_Doctor_BranchId",
                table: "OT_Doctor",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_OT_Doctor_Branch_BranchId",
                table: "OT_Doctor",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OT_Staff_Branch_BranchId",
                table: "OT_Staff",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
