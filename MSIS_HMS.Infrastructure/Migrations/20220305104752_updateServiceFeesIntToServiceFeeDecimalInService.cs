using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class updateServiceFeesIntToServiceFeeDecimalInService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceFees",
                table: "Service");

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceFee",
                table: "Service",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceFee",
                table: "Service");

            migrationBuilder.AddColumn<int>(
                name: "ServiceFees",
                table: "Service",
                type: "int",
                nullable: true);
        }
    }
}
