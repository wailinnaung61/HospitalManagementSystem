using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddVoucherSettingsInBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoPaidForOrder",
                table: "Branch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BatchNo",
                table: "Branch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BatchNoFormat",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseBatchNoFormat",
                table: "Branch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseVoucherFormatForDeliverOrder",
                table: "Branch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseVoucherFormatForOrder",
                table: "Branch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseVoucherFormatForPurchase",
                table: "Branch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VoucherFormatForDeliverOrder",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherFormatForOrder",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherFormatForPurchase",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoucherNoForDeliverOrder",
                table: "Branch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VoucherNoForOrder",
                table: "Branch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VoucherNoForPurchase",
                table: "Branch",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoPaidForOrder",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "BatchNo",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "BatchNoFormat",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "UseBatchNoFormat",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "UseVoucherFormatForDeliverOrder",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "UseVoucherFormatForOrder",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "UseVoucherFormatForPurchase",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherFormatForDeliverOrder",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherFormatForOrder",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherFormatForPurchase",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherNoForDeliverOrder",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherNoForOrder",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "VoucherNoForPurchase",
                table: "Branch");
        }
    }
}
