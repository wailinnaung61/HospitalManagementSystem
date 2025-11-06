using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddReferralFeeToReferrerAndOrderService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Referrer_Department_DepartmentId",
                table: "Referrer");

            migrationBuilder.DropIndex(
                name: "IX_Referrer_DepartmentId",
                table: "Referrer");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Referrer");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Referrer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                table: "Referrer",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FeeType",
                table: "Referrer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                table: "OrderService",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FeeType",
                table: "OrderService",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ReferralFee",
                table: "OrderService",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Referrer_BranchId",
                table: "Referrer",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Referrer_Branch_BranchId",
                table: "Referrer",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Referrer_Branch_BranchId",
                table: "Referrer");

            migrationBuilder.DropIndex(
                name: "IX_Referrer_BranchId",
                table: "Referrer");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Referrer");

            migrationBuilder.DropColumn(
                name: "Fee",
                table: "Referrer");

            migrationBuilder.DropColumn(
                name: "FeeType",
                table: "Referrer");

            migrationBuilder.DropColumn(
                name: "Fee",
                table: "OrderService");

            migrationBuilder.DropColumn(
                name: "FeeType",
                table: "OrderService");

            migrationBuilder.DropColumn(
                name: "ReferralFee",
                table: "OrderService");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Referrer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Referrer_DepartmentId",
                table: "Referrer",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Referrer_Department_DepartmentId",
                table: "Referrer",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
