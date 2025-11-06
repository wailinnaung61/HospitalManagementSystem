using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddTestIdintoLabTestDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LabTestId",
                table: "LabResultDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "LabResultDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LabResultDetail_LabTestId",
                table: "LabResultDetail",
                column: "LabTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabResultDetail_LabTest_LabTestId",
                table: "LabResultDetail",
                column: "LabTestId",
                principalTable: "LabTest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabResultDetail_LabTest_LabTestId",
                table: "LabResultDetail");

            migrationBuilder.DropIndex(
                name: "IX_LabResultDetail_LabTestId",
                table: "LabResultDetail");

            migrationBuilder.DropColumn(
                name: "LabTestId",
                table: "LabResultDetail");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "LabResultDetail");
        }
    }
}
