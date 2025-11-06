using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddColumnintoMedicalRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "MedicalRecord",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GCS",
                table: "MedicalRecord",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "History",
                table: "MedicalRecord",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SPO2",
                table: "MedicalRecord",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "MedicalRecord");

            migrationBuilder.DropColumn(
                name: "GCS",
                table: "MedicalRecord");

            migrationBuilder.DropColumn(
                name: "History",
                table: "MedicalRecord");

            migrationBuilder.DropColumn(
                name: "SPO2",
                table: "MedicalRecord");
        }
    }
}
