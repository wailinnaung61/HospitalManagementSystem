using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class changeColumTypeInIPDRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VouncherNo",
                table: "IPDRecord");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidDate",
                table: "IPDRecord",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "VoucherNo",
                table: "IPDRecord",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoucherNo",
                table: "IPDRecord");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidDate",
                table: "IPDRecord",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VouncherNo",
                table: "IPDRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
