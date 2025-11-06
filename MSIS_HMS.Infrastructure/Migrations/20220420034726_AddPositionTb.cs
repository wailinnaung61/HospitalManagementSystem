using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddPositionTb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {          
            migrationBuilder.AlterColumn<DateTime>(
                name: "DOA",
                table: "IPDRecord",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseTransferId",
                table: "Branch",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Position",
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
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.Id);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Ward_OutletId",
            //    table: "Ward",
            //    column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Branch_WarehouseTransferId",
                table: "Branch",
                column: "WarehouseTransferId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_WarehouseTransfer_WarehouseTransferId",
                table: "Branch",
                column: "WarehouseTransferId",
                principalTable: "WarehouseTransfer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Ward_Outlet_OutletId",
            //    table: "Ward",
            //    column: "OutletId",
            //    principalTable: "Outlet",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branch_WarehouseTransfer_WarehouseTransferId",
                table: "Branch");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Ward_Outlet_OutletId",
            //    table: "Ward");

            migrationBuilder.DropTable(
                name: "Position");

            //migrationBuilder.DropIndex(
            //    name: "IX_Ward_OutletId",
            //    table: "Ward");

            migrationBuilder.DropIndex(
                name: "IX_Branch_WarehouseTransferId",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "ToBranchId",
                table: "WarehouseTransfer");

            migrationBuilder.DropColumn(
                name: "WarehouseTransferId",
                table: "Branch");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DOA",
                table: "IPDRecord",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));
        }
    }
}
