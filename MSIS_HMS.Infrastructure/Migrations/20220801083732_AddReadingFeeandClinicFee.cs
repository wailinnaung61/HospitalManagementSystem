using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddReadingFeeandClinicFee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ReadingFee",
                table: "Doctor",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ClinicFee",
                table: "Branch",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadingFee",
                table: "Doctor");

            migrationBuilder.DropColumn(
                name: "ClinicFee",
                table: "Branch");
        }
    }
}
