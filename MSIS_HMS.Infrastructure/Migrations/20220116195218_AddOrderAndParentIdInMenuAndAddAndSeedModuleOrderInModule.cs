using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddOrderAndParentIdInMenuAndAddAndSeedModuleOrderInModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModuleOrder",
                table: "Module",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MenuOrder",
                table: "Menu",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Menu",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 1,
                column: "ModuleOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 2,
                column: "ModuleOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 3,
                column: "ModuleOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 4,
                column: "ModuleOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 5,
                column: "ModuleOrder",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 6,
                column: "ModuleOrder",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 7,
                column: "ModuleOrder",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 8,
                column: "ModuleOrder",
                value: 8);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModuleOrder",
                table: "Module");

            migrationBuilder.DropColumn(
                name: "MenuOrder",
                table: "Menu");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Menu");
        }
    }
}
