using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddColumnintoOperationTheatreandOTAneasthetist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AneasthetistType",
                table: "OperationTreater",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OTEndTime",
                table: "OperationTreater",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OTStartTime",
                table: "OperationTreater",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationProcedure",
                table: "OperationTreater",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostOPDiagnosis",
                table: "OperationTreater",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreOPDiagnosis",
                table: "OperationTreater",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SentToPathology",
                table: "OperationTreater",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OT_Anaesthetist",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    DoctorId = table.Column<int>(nullable: false),
                    OTDoctorTypeEnum = table.Column<int>(nullable: false),
                    Fee = table.Column<decimal>(nullable: false),
                    OperationTreaterId = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OT_Anaesthetist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OT_Anaesthetist_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OT_Anaesthetist_OperationTreater_OperationTreaterId",
                        column: x => x.OperationTreaterId,
                        principalTable: "OperationTreater",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OT_Anaesthetist_DoctorId",
                table: "OT_Anaesthetist",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_OT_Anaesthetist_OperationTreaterId",
                table: "OT_Anaesthetist",
                column: "OperationTreaterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OT_Anaesthetist");

            migrationBuilder.DropColumn(
                name: "AneasthetistType",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "OTEndTime",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "OTStartTime",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "OperationProcedure",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "PostOPDiagnosis",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "PreOPDiagnosis",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "SentToPathology",
                table: "OperationTreater");
        }
    }
}
