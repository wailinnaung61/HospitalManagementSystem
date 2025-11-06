using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddVisitNoFormatToBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VisitNo",
                table: "Visit",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VisitNo",
                table: "Branch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VisitNoFormat",
                table: "Branch",
                nullable: false,
                defaultValue: "000000");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitNo",
                table: "Visit");

            migrationBuilder.DropColumn(
                name: "VisitNo",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VisitNoFormat",
                table: "Branch");
        }
    }
}
