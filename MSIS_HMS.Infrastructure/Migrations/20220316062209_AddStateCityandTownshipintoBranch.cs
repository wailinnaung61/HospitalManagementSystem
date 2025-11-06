using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddStateCityandTownshipintoBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "Township",
                table: "Branch");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Branch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "Branch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TownshipId",
                table: "Branch",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "TownshipId",
                table: "Branch");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Branch",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Township",
                table: "Branch",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
