using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddOTDoctorandOTStaff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiefSurginDoctorId",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "ChiefSurginFee",
                table: "OperationTreater");

            migrationBuilder.AddColumn<int>(
                name: "ChiefSurgeonDoctorId",
                table: "OperationTreater",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ChiefSurgeonFee",
                table: "OperationTreater",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "OT_Doctor",
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
                    DoctorId = table.Column<int>(nullable: false),
                    OTDoctorTypeEnum = table.Column<int>(nullable: false),
                    Fee = table.Column<decimal>(nullable: false),
                    OperationTreaterId = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OT_Doctor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OT_Doctor_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OT_Doctor_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OT_Doctor_OperationTreater_OperationTreaterId",
                        column: x => x.OperationTreaterId,
                        principalTable: "OperationTreater",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OT_Staff",
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
                    StaffId = table.Column<int>(nullable: false),
                    OTStaffTypeEnum = table.Column<int>(nullable: false),
                    Fee = table.Column<decimal>(nullable: false),
                    OperationTreaterId = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OT_Staff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OT_Staff_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OT_Staff_OperationTreater_OperationTreaterId",
                        column: x => x.OperationTreaterId,
                        principalTable: "OperationTreater",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OT_Staff_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OT_Doctor_BranchId",
                table: "OT_Doctor",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OT_Doctor_DoctorId",
                table: "OT_Doctor",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_OT_Doctor_OperationTreaterId",
                table: "OT_Doctor",
                column: "OperationTreaterId");

            migrationBuilder.CreateIndex(
                name: "IX_OT_Staff_BranchId",
                table: "OT_Staff",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OT_Staff_OperationTreaterId",
                table: "OT_Staff",
                column: "OperationTreaterId");

            migrationBuilder.CreateIndex(
                name: "IX_OT_Staff_StaffId",
                table: "OT_Staff",
                column: "StaffId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OT_Doctor");

            migrationBuilder.DropTable(
                name: "OT_Staff");

            migrationBuilder.DropColumn(
                name: "ChiefSurgeonDoctorId",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "ChiefSurgeonFee",
                table: "OperationTreater");

            migrationBuilder.AddColumn<int>(
                name: "ChiefSurginDoctorId",
                table: "OperationTreater",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ChiefSurginFee",
                table: "OperationTreater",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
