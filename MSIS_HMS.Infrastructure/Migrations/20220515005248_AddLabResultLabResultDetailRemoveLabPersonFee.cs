using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddLabResultLabResultDetailRemoveLabPersonFee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabPersonFee");

            migrationBuilder.AddColumn<int>(
                name: "LabResultId",
                table: "LabOrderTest",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LabResult",
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
                    ResultNo = table.Column<string>(nullable: false),
                    PatientId = table.Column<int>(nullable: false),
                    LabTestId = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    TechnicianId = table.Column<int>(nullable: true),
                    TechnicianFee = table.Column<decimal>(nullable: false),
                    TechnicianFeeType = table.Column<int>(nullable: false),
                    ConsultantId = table.Column<int>(nullable: true),
                    ConsultantFee = table.Column<decimal>(nullable: false),
                    ConsultantFeeType = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabResult_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabResult_LabPerson_ConsultantId",
                        column: x => x.ConsultantId,
                        principalTable: "LabPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabResult_LabTest_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabResult_LabPerson_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "LabPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabResultDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    LabResultId = table.Column<int>(nullable: false),
                    LabUnit = table.Column<string>(nullable: true),
                    Result = table.Column<string>(nullable: true),
                    MinRange = table.Column<double>(nullable: true),
                    MaxRange = table.Column<double>(nullable: true),
                    SelectList = table.Column<string>(nullable: true),
                    LabResultType = table.Column<int>(nullable: false),
                    IsTitle = table.Column<bool>(nullable: false),
                    IsPrinted = table.Column<bool>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabResultDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabResultDetail_LabResult_LabResultId",
                        column: x => x.LabResultId,
                        principalTable: "LabResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabOrderTest_LabResultId",
                table: "LabOrderTest",
                column: "LabResultId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResult_BranchId",
                table: "LabResult",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResult_ConsultantId",
                table: "LabResult",
                column: "ConsultantId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResult_LabTestId",
                table: "LabResult",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResult_TechnicianId",
                table: "LabResult",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResultDetail_LabResultId",
                table: "LabResultDetail",
                column: "LabResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrderTest_LabResult_LabResultId",
                table: "LabOrderTest",
                column: "LabResultId",
                principalTable: "LabResult",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrderTest_LabResult_LabResultId",
                table: "LabOrderTest");

            migrationBuilder.DropTable(
                name: "LabResultDetail");

            migrationBuilder.DropTable(
                name: "LabResult");

            migrationBuilder.DropIndex(
                name: "IX_LabOrderTest_LabResultId",
                table: "LabOrderTest");

            migrationBuilder.DropColumn(
                name: "LabResultId",
                table: "LabOrderTest");

            migrationBuilder.CreateTable(
                name: "LabPersonFee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeeType = table.Column<int>(type: "int", nullable: false),
                    LabOrderTestId = table.Column<int>(type: "int", nullable: false),
                    LabPersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabPersonFee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabPersonFee_LabOrderTest_LabOrderTestId",
                        column: x => x.LabOrderTestId,
                        principalTable: "LabOrderTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabPersonFee_LabPerson_LabPersonId",
                        column: x => x.LabPersonId,
                        principalTable: "LabPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabPersonFee_LabOrderTestId",
                table: "LabPersonFee",
                column: "LabOrderTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabPersonFee_LabPersonId",
                table: "LabPersonFee",
                column: "LabPersonId");
        }
    }
}
