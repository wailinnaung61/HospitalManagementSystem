using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddIPDPaymentandDepartmentColumnintoIPDRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "IPDRecord",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "IPDPayment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    PaymentType = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    PaidBy = table.Column<string>(nullable: true),
                    IPDRecordId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPDPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPDPayment_IPDRecord_IPDRecordId",
                        column: x => x.IPDRecordId,
                        principalTable: "IPDRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IPDRecord_DepartmentId",
                table: "IPDRecord",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDPayment_IPDRecordId",
                table: "IPDPayment",
                column: "IPDRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_IPDRecord_Department_DepartmentId",
                table: "IPDRecord",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPDRecord_Department_DepartmentId",
                table: "IPDRecord");

            migrationBuilder.DropTable(
                name: "IPDPayment");

            migrationBuilder.DropIndex(
                name: "IX_IPDRecord_DepartmentId",
                table: "IPDRecord");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "IPDRecord");
        }
    }
}
