using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddIsDashboardintoMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDashboard",
                table: "Menu",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_LabResult_PatientId",
                table: "LabResult",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabResult_Patient_PatientId",
                table: "LabResult",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabResult_Patient_PatientId",
                table: "LabResult");

            migrationBuilder.DropIndex(
                name: "IX_LabResult_PatientId",
                table: "LabResult");

            migrationBuilder.DropColumn(
                name: "IsDashboard",
                table: "Menu");
        }
    }
}
