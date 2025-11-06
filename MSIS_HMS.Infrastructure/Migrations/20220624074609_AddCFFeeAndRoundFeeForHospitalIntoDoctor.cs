using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddCFFeeAndRoundFeeForHospitalIntoDoctor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CFFeeForHospital",
                table: "Doctor",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RoundFeeForHospital",
                table: "Doctor",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CFFeeForHospital",
                table: "Doctor");

            migrationBuilder.DropColumn(
                name: "RoundFeeForHospital",
                table: "Doctor");
        }
    }
}
