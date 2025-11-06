using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class DropCFFeeForHospital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CFFeeForHospital",
                table: "Branch");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CFFeeForHospital",
                table: "Branch",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
