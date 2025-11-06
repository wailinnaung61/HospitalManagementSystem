using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class RenameSortOrderInLabOrderTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortLabOrder",
                table: "LabOrderTest");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "LabOrderTest",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "LabOrderTest");

            migrationBuilder.AddColumn<int>(
                name: "SortLabOrder",
                table: "LabOrderTest",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
