using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class RemoveDepartmentAddBranchToServiceAndServiceType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_Department_DepartmentId",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceType_Department_DepartmentId",
                table: "ServiceType");

            migrationBuilder.DropIndex(
                name: "IX_ServiceType_DepartmentId",
                table: "ServiceType");

            migrationBuilder.DropIndex(
                name: "IX_Service_DepartmentId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Service");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "ServiceType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Service",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceType_BranchId",
                table: "ServiceType",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_BranchId",
                table: "Service",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Branch_BranchId",
                table: "Service",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceType_Branch_BranchId",
                table: "ServiceType",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_Branch_BranchId",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceType_Branch_BranchId",
                table: "ServiceType");

            migrationBuilder.DropIndex(
                name: "IX_ServiceType_BranchId",
                table: "ServiceType");

            migrationBuilder.DropIndex(
                name: "IX_Service_BranchId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Service");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ServiceType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "ServiceType",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Service",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceType_DepartmentId",
                table: "ServiceType",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_DepartmentId",
                table: "Service",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Department_DepartmentId",
                table: "Service",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceType_Department_DepartmentId",
                table: "ServiceType",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
