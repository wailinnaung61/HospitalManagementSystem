using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class EditCodetoQRcodeandAddBarCodeColumnInPatient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Patient");

            migrationBuilder.AddColumn<string>(
                name: "BarCode",
                table: "Patient",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRCode",
                table: "Patient",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarCode",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "QRCode",
                table: "Patient");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
