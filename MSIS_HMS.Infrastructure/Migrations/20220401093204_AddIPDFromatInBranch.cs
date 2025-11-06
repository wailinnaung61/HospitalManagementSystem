using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddIPDFromatInBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoPaidForIPD",
                table: "Branch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseVoucherFormatForIPD",
                table: "Branch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VoucherFormatForIPD",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoucherNoForIPD",
                table: "Branch",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoPaidForIPD",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "UseVoucherFormatForIPD",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherFormatForIPD",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherNoForIPD",
                table: "Branch");
        }
    }
}
