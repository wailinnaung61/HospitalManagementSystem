using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddBranchIdToAspNetUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nurse_Department_departmentId",
                table: "Nurse");

            migrationBuilder.RenameColumn(
                name: "departmentId",
                table: "Nurse",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Nurse_departmentId",
                table: "Nurse",
                newName: "IX_Nurse_DepartmentId");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BranchId",
                table: "AspNetUsers",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Branch_BranchId",
                table: "AspNetUsers",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Nurse_Department_DepartmentId",
                table: "Nurse",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Branch_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Nurse_Department_DepartmentId",
                table: "Nurse");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Nurse",
                newName: "departmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Nurse_DepartmentId",
                table: "Nurse",
                newName: "IX_Nurse_departmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nurse_Department_departmentId",
                table: "Nurse",
                column: "departmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
