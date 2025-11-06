using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddVisitAndAppointmentAndUpdatePatient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patient_Department_DepartmentId",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_Patient_DepartmentId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "Patient");

            migrationBuilder.AddColumn<string>(
                name: "Allergies",
                table: "Patient",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodType",
                table: "Patient",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Guardian",
                table: "Patient",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                table: "Patient",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceOfBirth",
                table: "Patient",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferredBy",
                table: "Patient",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReferredDate",
                table: "Patient",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReferrerId",
                table: "Patient",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Religion",
                table: "Patient",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Patient",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientRegFormat",
                table: "Branch",
                nullable: false,
                defaultValue: "P-000000");

            migrationBuilder.AddColumn<int>(
                name: "PatientRegNo",
                table: "Branch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AppointmentType",
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
                    Type = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentType_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisitType",
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
                    Type = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitType_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
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
                    PatientId = table.Column<int>(nullable: false),
                    DoctorId = table.Column<int>(nullable: false),
                    AppointmentTypeId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointment_AppointmentType_AppointmentTypeId",
                        column: x => x.AppointmentTypeId,
                        principalTable: "AppointmentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Visit",
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
                    PatientId = table.Column<int>(nullable: false),
                    DoctorId = table.Column<int>(nullable: false),
                    VisitTypeId = table.Column<int>(nullable: false),
                    ReasonForVisit = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visit_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visit_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visit_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visit_VisitType_VisitTypeId",
                        column: x => x.VisitTypeId,
                        principalTable: "VisitType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patient_ReferrerId",
                table: "Patient",
                column: "ReferrerId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_AppointmentTypeId",
                table: "Appointment",
                column: "AppointmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_BranchId",
                table: "Appointment",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_DoctorId",
                table: "Appointment",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PatientId",
                table: "Appointment",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentType_BranchId",
                table: "AppointmentType",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Visit_BranchId",
                table: "Visit",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Visit_DoctorId",
                table: "Visit",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Visit_PatientId",
                table: "Visit",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Visit_VisitTypeId",
                table: "Visit",
                column: "VisitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitType_BranchId",
                table: "VisitType",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_Referrer_ReferrerId",
                table: "Patient",
                column: "ReferrerId",
                principalTable: "Referrer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patient_Referrer_ReferrerId",
                table: "Patient");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "Visit");

            migrationBuilder.DropTable(
                name: "AppointmentType");

            migrationBuilder.DropTable(
                name: "VisitType");

            migrationBuilder.DropIndex(
                name: "IX_Patient_ReferrerId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Allergies",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Guardian",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "PlaceOfBirth",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "ReferredBy",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "ReferredDate",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "ReferrerId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Religion",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "PatientRegFormat",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "PatientRegNo",
                table: "Branch");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Patient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_DepartmentId",
                table: "Patient",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_Department_DepartmentId",
                table: "Patient",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
