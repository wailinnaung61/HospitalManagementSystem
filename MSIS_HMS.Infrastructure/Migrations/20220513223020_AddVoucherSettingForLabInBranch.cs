using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddVoucherSettingForLabInBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoPaidForLab",
                table: "Branch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseVoucherFormatForLab",
                table: "Branch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VoucherFormatForLab",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoucherNoForLab",
                table: "Branch",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoPaidForLab",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "UseVoucherFormatForLab",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherFormatForLab",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherNoForLab",
                table: "Branch");
        }
    }
}
