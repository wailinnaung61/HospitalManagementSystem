using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddOperationTreaterOperationServiceandItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationTreater",
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
                    OperationDate = table.Column<DateTime>(nullable: false),
                    PatientId = table.Column<int>(nullable: false),
                    OutletId = table.Column<int>(nullable: false),
                    ChiefSurginDoctorId = table.Column<int>(nullable: false),
                    ChiefSurginFee = table.Column<decimal>(nullable: false),
                    OperationRoomId = table.Column<int>(nullable: false),
                    OpeartionTypeId = table.Column<int>(nullable: false),
                    PaidBy = table.Column<string>(nullable: true),
                    IsPaid = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    DoctorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationTreater", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationTreater_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationTreater_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationTreater_Outlet_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationTreater_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperationItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationTreaterId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    QtyInSmallestUnit = table.Column<int>(nullable: false),
                    IsFOC = table.Column<bool>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationItem_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationItem_OperationTreater_OperationTreaterId",
                        column: x => x.OperationTreaterId,
                        principalTable: "OperationTreater",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperationService",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationTreaterId = table.Column<int>(nullable: false),
                    ServiceId = table.Column<int>(nullable: false),
                    Fee = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    IsFOC = table.Column<bool>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationService_OperationTreater_OperationTreaterId",
                        column: x => x.OperationTreaterId,
                        principalTable: "OperationTreater",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationService_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationItem_ItemId",
                table: "OperationItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationItem_OperationTreaterId",
                table: "OperationItem",
                column: "OperationTreaterId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationItem_UnitId",
                table: "OperationItem",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationService_OperationTreaterId",
                table: "OperationService",
                column: "OperationTreaterId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationService_ServiceId",
                table: "OperationService",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTreater_BranchId",
                table: "OperationTreater",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTreater_DoctorId",
                table: "OperationTreater",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTreater_OutletId",
                table: "OperationTreater",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTreater_PatientId",
                table: "OperationTreater",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationItem");

            migrationBuilder.DropTable(
                name: "OperationService");

            migrationBuilder.DropTable(
                name: "OperationTreater");
        }
    }
}
