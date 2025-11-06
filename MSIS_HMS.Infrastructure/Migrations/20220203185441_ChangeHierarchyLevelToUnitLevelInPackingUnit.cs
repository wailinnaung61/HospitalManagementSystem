using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class ChangeHierarchyLevelToUnitLevelInPackingUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HierarchyLevel",
                table: "PackingUnit");

            migrationBuilder.AddColumn<int>(
                name: "UnitLevel",
                table: "PackingUnit",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitLevel",
                table: "PackingUnit");

            migrationBuilder.AddColumn<int>(
                name: "HierarchyLevel",
                table: "PackingUnit",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
