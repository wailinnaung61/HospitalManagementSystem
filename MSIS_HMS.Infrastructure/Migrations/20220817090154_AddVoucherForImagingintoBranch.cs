using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddVoucherForImagingintoBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseVoucherFormatForImaging",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherFormatForImaging",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoucherNoForImaging",
                table: "Branch",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseVoucherFormatForImaging",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherFormatForImaging",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherNoForImaging",
                table: "Branch");
        }
    }
}
