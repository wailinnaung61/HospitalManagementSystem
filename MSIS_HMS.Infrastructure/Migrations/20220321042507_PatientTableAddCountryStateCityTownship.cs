using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class PatientTableAddCountryStateCityTownship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Township",
                table: "Patient");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Patient",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Patient",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "Patient",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TownshipId",
                table: "Patient",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "TownshipId",
                table: "Patient");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Township",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
