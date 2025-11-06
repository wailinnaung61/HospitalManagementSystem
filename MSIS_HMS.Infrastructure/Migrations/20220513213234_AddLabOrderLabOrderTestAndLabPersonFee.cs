using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddLabOrderLabOrderTestAndLabPersonFee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LabOrder",
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
                    Date = table.Column<DateTime>(nullable: false),
                    VoucherNo = table.Column<string>(nullable: false),
                    PatientId = table.Column<int>(nullable: false),
                    Tax = table.Column<decimal>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    PaidDate = table.Column<DateTime>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabOrder_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabOrder_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabOrderTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderId = table.Column<int>(nullable: false),
                    LabTestId = table.Column<int>(nullable: false),
                    ReferrerId = table.Column<int>(nullable: true),
                    FeeType = table.Column<int>(nullable: false),
                    Fee = table.Column<decimal>(nullable: false),
                    ReferralFee = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    SortLabOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabOrderTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabOrderTest_LabOrder_LabOrderId",
                        column: x => x.LabOrderId,
                        principalTable: "LabOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabOrderTest_LabTest_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabOrderTest_Referrer_ReferrerId",
                        column: x => x.ReferrerId,
                        principalTable: "Referrer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabPersonFee",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fee = table.Column<decimal>(nullable: false),
                    FeeType = table.Column<int>(nullable: false),
                    LabPersonId = table.Column<int>(nullable: false),
                    LabOrderTestId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabPersonFee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabPersonFee_LabOrderTest_LabOrderTestId",
                        column: x => x.LabOrderTestId,
                        principalTable: "LabOrderTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabPersonFee_LabPerson_LabPersonId",
                        column: x => x.LabPersonId,
                        principalTable: "LabPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabOrder_BranchId",
                table: "LabOrder",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrder_PatientId",
                table: "LabOrder",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrderTest_LabOrderId",
                table: "LabOrderTest",
                column: "LabOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrderTest_LabTestId",
                table: "LabOrderTest",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrderTest_ReferrerId",
                table: "LabOrderTest",
                column: "ReferrerId");

            migrationBuilder.CreateIndex(
                name: "IX_LabPersonFee_LabOrderTestId",
                table: "LabPersonFee",
                column: "LabOrderTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabPersonFee_LabPersonId",
                table: "LabPersonFee",
                column: "LabPersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabPersonFee");

            migrationBuilder.DropTable(
                name: "LabOrderTest");

            migrationBuilder.DropTable(
                name: "LabOrder");
        }
    }
}
