using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class modifyDoctortb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CFFee",
                table: "Doctor",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "Doctor",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
            
            migrationBuilder.AlterColumn<int>(
                name: "Code",
                table: "Doctor",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "string");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CFFee",
                table: "Doctor",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "Doctor",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
            
            migrationBuilder.AlterColumn<int>(
                name: "Code",
                table: "Doctor",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "string");
        }
    }
}
