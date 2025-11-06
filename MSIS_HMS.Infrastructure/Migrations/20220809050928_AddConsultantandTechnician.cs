using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddConsultantandTechnician : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConsultantFee",
                table: "LabResultDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ConsultantFeeType",
                table: "LabResultDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ConsultantId",
                table: "LabResultDetail",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TechnicianFee",
                table: "LabResultDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TechnicianFeeType",
                table: "LabResultDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TechnicianId",
                table: "LabResultDetail",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabResultDetail_ConsultantId",
                table: "LabResultDetail",
                column: "ConsultantId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResultDetail_TechnicianId",
                table: "LabResultDetail",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabResultDetail_LabPerson_ConsultantId",
                table: "LabResultDetail",
                column: "ConsultantId",
                principalTable: "LabPerson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabResultDetail_LabPerson_TechnicianId",
                table: "LabResultDetail",
                column: "TechnicianId",
                principalTable: "LabPerson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabResultDetail_LabPerson_ConsultantId",
                table: "LabResultDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_LabResultDetail_LabPerson_TechnicianId",
                table: "LabResultDetail");

            migrationBuilder.DropIndex(
                name: "IX_LabResultDetail_ConsultantId",
                table: "LabResultDetail");

            migrationBuilder.DropIndex(
                name: "IX_LabResultDetail_TechnicianId",
                table: "LabResultDetail");

            migrationBuilder.DropColumn(
                name: "ConsultantFee",
                table: "LabResultDetail");

            migrationBuilder.DropColumn(
                name: "ConsultantFeeType",
                table: "LabResultDetail");

            migrationBuilder.DropColumn(
                name: "ConsultantId",
                table: "LabResultDetail");

            migrationBuilder.DropColumn(
                name: "TechnicianFee",
                table: "LabResultDetail");

            migrationBuilder.DropColumn(
                name: "TechnicianFeeType",
                table: "LabResultDetail");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "LabResultDetail");
        }
    }
}
