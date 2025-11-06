using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddMedicalRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicalRecord",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    BPSystolic = table.Column<string>(nullable: true),
                    BPDiastolic = table.Column<string>(nullable: true),
                    Temperature = table.Column<string>(nullable: true),
                    Pulse = table.Column<string>(nullable: true),
                    RespiratoryRate = table.Column<string>(nullable: true),
                    Weight = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    DepartmentType = table.Column<int>(nullable: false),
                    DoctorNotes = table.Column<string>(nullable: true),
                    DoctorNotesImg = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    BranchId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecord_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalRecord_Visit_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientDiagnostic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicalRecordId = table.Column<int>(nullable: false),
                    DiagnosticId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Detail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDiagnostic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientDiagnostic_Diagnostic_DiagnosticId",
                        column: x => x.DiagnosticId,
                        principalTable: "Diagnostic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientDiagnostic_MedicalRecord_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientSymptom",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicalRecordId = table.Column<int>(nullable: false),
                    SymptomId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Detail = table.Column<string>(nullable: true),
                    FromDate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientSymptom", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientSymptom_MedicalRecord_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientSymptom_Symptom_SymptomId",
                        column: x => x.SymptomId,
                        principalTable: "Symptom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prescription",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicalRecordId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: true),
                    Drug = table.Column<string>(nullable: true),
                    DirectionsForUse = table.Column<string>(nullable: true),
                    Day = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescription_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prescription_MedicalRecord_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_BranchId",
                table: "MedicalRecord",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_VisitId",
                table: "MedicalRecord",
                column: "VisitId",
                unique: true,
                filter: "[VisitId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiagnostic_DiagnosticId",
                table: "PatientDiagnostic",
                column: "DiagnosticId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiagnostic_MedicalRecordId",
                table: "PatientDiagnostic",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientSymptom_MedicalRecordId",
                table: "PatientSymptom",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientSymptom_SymptomId",
                table: "PatientSymptom",
                column: "SymptomId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_ItemId",
                table: "Prescription",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_MedicalRecordId",
                table: "Prescription",
                column: "MedicalRecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientDiagnostic");

            migrationBuilder.DropTable(
                name: "PatientSymptom");

            migrationBuilder.DropTable(
                name: "Prescription");

            migrationBuilder.DropTable(
                name: "MedicalRecord");
        }
    }
}
