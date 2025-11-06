using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddLabTestAndLabTestDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LabTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsLabReport = table.Column<bool>(nullable: false),
                    Category = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    ReferrerId = table.Column<int>(nullable: true),
                    ReferralFee = table.Column<decimal>(nullable: false),
                    ReferralFeeType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTest_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabTestDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    LabTestId = table.Column<int>(nullable: false),
                    LabUnit = table.Column<string>(nullable: true),
                    MinRange = table.Column<double>(nullable: true),
                    MaxRange = table.Column<double>(nullable: true),
                    SelectList = table.Column<string>(nullable: true),
                    LabResultType = table.Column<int>(nullable: false),
                    IsTitle = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    IsDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTestDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTestDetail_LabTest_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabTest_BranchId",
                table: "LabTest",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestDetail_LabTestId",
                table: "LabTestDetail",
                column: "LabTestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabTestDetail");

            migrationBuilder.DropTable(
                name: "LabTest");
        }
    }
}
