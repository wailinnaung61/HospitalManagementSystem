using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddBranchToStaffAndSpecialityAndNurse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Staff",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Speciality",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Nurse",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Staff_BranchId",
                table: "Staff",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Speciality_BranchId",
                table: "Speciality",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Nurse_BranchId",
                table: "Nurse",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nurse_Branch_BranchId",
                table: "Nurse",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Speciality_Branch_BranchId",
                table: "Speciality",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Branch_BranchId",
                table: "Staff",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nurse_Branch_BranchId",
                table: "Nurse");

            migrationBuilder.DropForeignKey(
                name: "FK_Speciality_Branch_BranchId",
                table: "Speciality");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_Branch_BranchId",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Staff_BranchId",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Speciality_BranchId",
                table: "Speciality");

            migrationBuilder.DropIndex(
                name: "IX_Nurse_BranchId",
                table: "Nurse");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Speciality");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Nurse");
        }
    }
}
