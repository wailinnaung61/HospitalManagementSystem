using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddBranchToDoctorAndAddStatusAndReasonForCancellationToVisit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReasonForCancellation",
                table: "Visit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Visit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Doctor",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_BranchId",
                table: "Doctor",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctor_Branch_BranchId",
                table: "Doctor",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctor_Branch_BranchId",
                table: "Doctor");

            migrationBuilder.DropIndex(
                name: "IX_Doctor_BranchId",
                table: "Doctor");

            migrationBuilder.DropColumn(
                name: "ReasonForCancellation",
                table: "Visit");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Visit");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Doctor");
        }
    }
}
