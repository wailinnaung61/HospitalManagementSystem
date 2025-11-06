using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddVoucherSettingForLabResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseVoucherFormatForLabResult",
                table: "Branch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VoucherFormatForLabResult",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoucherNoForLabResult",
                table: "Branch",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseVoucherFormatForLabResult",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherFormatForLabResult",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherNoForLabResult",
                table: "Branch");
        }
    }
}
