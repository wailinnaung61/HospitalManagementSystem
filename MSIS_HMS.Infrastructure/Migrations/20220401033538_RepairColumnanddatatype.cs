using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class RepairColumnanddatatype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IPDRecord",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    DOA = table.Column<DateTime>(nullable: true),
                    DODC = table.Column<DateTime>(nullable: true),
                    PatientId = table.Column<int>(nullable: false),
                    VouncherNo = table.Column<string>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    PaidDate = table.Column<DateTime>(nullable: false),
                    PaymentType = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    RoomId = table.Column<int>(nullable: true),
                    BedId = table.Column<int>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Discount = table.Column<int>(nullable: true),
                    Tax = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPDRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPDRecord_Bed_BedId",
                        column: x => x.BedId,
                        principalTable: "Bed",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPDRecord_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPDRecord_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Food",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FoodCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Food", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Food_FoodCategory_FoodCategoryId",
                        column: x => x.FoodCategoryId,
                        principalTable: "FoodCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IPDOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    VoucherNo = table.Column<string>(nullable: false),
                    IPDRecordId = table.Column<int>(nullable: false),
                    CFFee = table.Column<decimal>(nullable: false),
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
                    table.PrimaryKey("PK_IPDOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPDOrder_IPDRecord_IPDRecordId",
                        column: x => x.IPDRecordId,
                        principalTable: "IPDRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IPDAllotment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(nullable: false),
                    BedId = table.Column<int>(nullable: true),
                    IPDOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPDAllotment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPDAllotment_Bed_BedId",
                        column: x => x.BedId,
                        principalTable: "Bed",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPDAllotment_IPDOrder_IPDOrderId",
                        column: x => x.IPDOrderId,
                        principalTable: "IPDOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPDAllotment_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IPDFood",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<int>(nullable: false),
                    IPDOrderId = table.Column<int>(nullable: false),
                    IsFOC = table.Column<bool>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPDFood", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPDFood_Food_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPDFood_IPDOrder_IPDOrderId",
                        column: x => x.IPDOrderId,
                        principalTable: "IPDOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IPDOrderItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IPDOrderId = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_IPDOrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPDOrderItem_IPDOrder_IPDOrderId",
                        column: x => x.IPDOrderId,
                        principalTable: "IPDOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPDOrderItem_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPDOrderItem_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IPDOrderService",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IPDOrderId = table.Column<int>(nullable: false),
                    ServiceId = table.Column<int>(nullable: false),
                    FeeType = table.Column<int>(nullable: false),
                    Fee = table.Column<decimal>(nullable: false),
                    ReferralFee = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    IsFOC = table.Column<bool>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPDOrderService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPDOrderService_IPDOrder_IPDOrderId",
                        column: x => x.IPDOrderId,
                        principalTable: "IPDOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPDOrderService_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IPDRound",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fee = table.Column<decimal>(nullable: false),
                    IsFOC = table.Column<bool>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    DoctorId = table.Column<int>(nullable: false),
                    IPDOrderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPDRound", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPDRound_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPDRound_IPDOrder_IPDOrderID",
                        column: x => x.IPDOrderID,
                        principalTable: "IPDOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IPDStaff",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Fee = table.Column<decimal>(nullable: false),
                    IsFOC = table.Column<bool>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    StaffId = table.Column<int>(nullable: false),
                    IPDOrderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPDStaff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPDStaff_IPDOrder_IPDOrderID",
                        column: x => x.IPDOrderID,
                        principalTable: "IPDOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IPDStaff_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Food_FoodCategoryId",
                table: "Food",
                column: "FoodCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDAllotment_BedId",
                table: "IPDAllotment",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDAllotment_IPDOrderId",
                table: "IPDAllotment",
                column: "IPDOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDAllotment_RoomId",
                table: "IPDAllotment",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDFood_FoodId",
                table: "IPDFood",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDFood_IPDOrderId",
                table: "IPDFood",
                column: "IPDOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDOrder_IPDRecordId",
                table: "IPDOrder",
                column: "IPDRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDOrderItem_IPDOrderId",
                table: "IPDOrderItem",
                column: "IPDOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDOrderItem_ItemId",
                table: "IPDOrderItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDOrderItem_UnitId",
                table: "IPDOrderItem",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDOrderService_IPDOrderId",
                table: "IPDOrderService",
                column: "IPDOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDOrderService_ServiceId",
                table: "IPDOrderService",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDRecord_BedId",
                table: "IPDRecord",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDRecord_PatientId",
                table: "IPDRecord",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDRecord_RoomId",
                table: "IPDRecord",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDRound_DoctorId",
                table: "IPDRound",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDRound_IPDOrderID",
                table: "IPDRound",
                column: "IPDOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_IPDStaff_IPDOrderID",
                table: "IPDStaff",
                column: "IPDOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_IPDStaff_StaffId",
                table: "IPDStaff",
                column: "StaffId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IPDAllotment");

            migrationBuilder.DropTable(
                name: "IPDFood");

            migrationBuilder.DropTable(
                name: "IPDOrderItem");

            migrationBuilder.DropTable(
                name: "IPDOrderService");

            migrationBuilder.DropTable(
                name: "IPDRound");

            migrationBuilder.DropTable(
                name: "IPDStaff");

            migrationBuilder.DropTable(
                name: "Food");

            migrationBuilder.DropTable(
                name: "IPDOrder");

            migrationBuilder.DropTable(
                name: "FoodCategory");

            migrationBuilder.DropTable(
                name: "IPDRecord");
        }
    }
}
