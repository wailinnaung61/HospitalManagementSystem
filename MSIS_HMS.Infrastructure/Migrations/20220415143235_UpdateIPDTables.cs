using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class UpdateIPDTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPDAllotment_IPDOrder_IPDOrderId",
                table: "IPDAllotment");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDFood_IPDOrder_IPDOrderId",
                table: "IPDFood");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDOrderItem_IPDOrder_IPDOrderId",
                table: "IPDOrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDOrderService_IPDOrder_IPDOrderId",
                table: "IPDOrderService");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDRound_IPDOrder_IPDOrderID",
                table: "IPDRound");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "IPDFood");

            migrationBuilder.RenameColumn(
                name: "IPDOrderID",
                table: "IPDRound",
                newName: "IPDOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_IPDRound_IPDOrderID",
                table: "IPDRound",
                newName: "IX_IPDRound_IPDOrderId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "IPDStaff",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderId",
                table: "IPDRound",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "IPDRound",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IPDRecordID",
                table: "IPDRound",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Tax",
                table: "IPDRecord",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "IPDRecord",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "IPDRecord",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "IPDRecord",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderId",
                table: "IPDOrderService",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "IPDOrderService",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IPDRecordId",
                table: "IPDOrderService",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderId",
                table: "IPDOrderItem",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "IPDOrderItem",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IPDRecordId",
                table: "IPDOrderItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderId",
                table: "IPDFood",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "IPDFood",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IPDRecordId",
                table: "IPDFood",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Qty",
                table: "IPDFood",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderId",
                table: "IPDAllotment",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "IPDAllotment",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IPDRecordId",
                table: "IPDAllotment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IPDRound_IPDRecordID",
                table: "IPDRound",
                column: "IPDRecordID");

            migrationBuilder.CreateIndex(
                name: "IX_IPDOrderService_IPDRecordId",
                table: "IPDOrderService",
                column: "IPDRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDOrderItem_IPDRecordId",
                table: "IPDOrderItem",
                column: "IPDRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDFood_IPDRecordId",
                table: "IPDFood",
                column: "IPDRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDAllotment_IPDRecordId",
                table: "IPDAllotment",
                column: "IPDRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_IPDAllotment_IPDOrder_IPDOrderId",
                table: "IPDAllotment",
                column: "IPDOrderId",
                principalTable: "IPDOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDAllotment_IPDRecord_IPDRecordId",
                table: "IPDAllotment",
                column: "IPDRecordId",
                principalTable: "IPDRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDFood_IPDOrder_IPDOrderId",
                table: "IPDFood",
                column: "IPDOrderId",
                principalTable: "IPDOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDFood_IPDRecord_IPDRecordId",
                table: "IPDFood",
                column: "IPDRecordId",
                principalTable: "IPDRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDOrderItem_IPDOrder_IPDOrderId",
                table: "IPDOrderItem",
                column: "IPDOrderId",
                principalTable: "IPDOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDOrderItem_IPDRecord_IPDRecordId",
                table: "IPDOrderItem",
                column: "IPDRecordId",
                principalTable: "IPDRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDOrderService_IPDOrder_IPDOrderId",
                table: "IPDOrderService",
                column: "IPDOrderId",
                principalTable: "IPDOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDOrderService_IPDRecord_IPDRecordId",
                table: "IPDOrderService",
                column: "IPDRecordId",
                principalTable: "IPDRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDRound_IPDOrder_IPDOrderId",
                table: "IPDRound",
                column: "IPDOrderId",
                principalTable: "IPDOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDRound_IPDRecord_IPDRecordID",
                table: "IPDRound",
                column: "IPDRecordID",
                principalTable: "IPDRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPDAllotment_IPDOrder_IPDOrderId",
                table: "IPDAllotment");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDAllotment_IPDRecord_IPDRecordId",
                table: "IPDAllotment");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDFood_IPDOrder_IPDOrderId",
                table: "IPDFood");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDFood_IPDRecord_IPDRecordId",
                table: "IPDFood");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDOrderItem_IPDOrder_IPDOrderId",
                table: "IPDOrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDOrderItem_IPDRecord_IPDRecordId",
                table: "IPDOrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDOrderService_IPDOrder_IPDOrderId",
                table: "IPDOrderService");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDOrderService_IPDRecord_IPDRecordId",
                table: "IPDOrderService");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDRound_IPDOrder_IPDOrderId",
                table: "IPDRound");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDRound_IPDRecord_IPDRecordID",
                table: "IPDRound");

            migrationBuilder.DropIndex(
                name: "IX_IPDRound_IPDRecordID",
                table: "IPDRound");

            migrationBuilder.DropIndex(
                name: "IX_IPDOrderService_IPDRecordId",
                table: "IPDOrderService");

            migrationBuilder.DropIndex(
                name: "IX_IPDOrderItem_IPDRecordId",
                table: "IPDOrderItem");

            migrationBuilder.DropIndex(
                name: "IX_IPDFood_IPDRecordId",
                table: "IPDFood");

            migrationBuilder.DropIndex(
                name: "IX_IPDAllotment_IPDRecordId",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "IPDStaff");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "IPDRound");

            migrationBuilder.DropColumn(
                name: "IPDRecordID",
                table: "IPDRound");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "IPDRecord");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "IPDRecord");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "IPDOrderService");

            migrationBuilder.DropColumn(
                name: "IPDRecordId",
                table: "IPDOrderService");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "IPDOrderItem");

            migrationBuilder.DropColumn(
                name: "IPDRecordId",
                table: "IPDOrderItem");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "IPDFood");

            migrationBuilder.DropColumn(
                name: "IPDRecordId",
                table: "IPDFood");

            migrationBuilder.DropColumn(
                name: "Qty",
                table: "IPDFood");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "IPDRecordId",
                table: "IPDAllotment");

            migrationBuilder.RenameColumn(
                name: "IPDOrderId",
                table: "IPDRound",
                newName: "IPDOrderID");

            migrationBuilder.RenameIndex(
                name: "IX_IPDRound_IPDOrderId",
                table: "IPDRound",
                newName: "IX_IPDRound_IPDOrderID");

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderID",
                table: "IPDRound",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Tax",
                table: "IPDRecord",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<int>(
                name: "Discount",
                table: "IPDRecord",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderId",
                table: "IPDOrderService",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderId",
                table: "IPDOrderItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderId",
                table: "IPDFood",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "IPDFood",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderId",
                table: "IPDAllotment",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDAllotment_IPDOrder_IPDOrderId",
                table: "IPDAllotment",
                column: "IPDOrderId",
                principalTable: "IPDOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDFood_IPDOrder_IPDOrderId",
                table: "IPDFood",
                column: "IPDOrderId",
                principalTable: "IPDOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDOrderItem_IPDOrder_IPDOrderId",
                table: "IPDOrderItem",
                column: "IPDOrderId",
                principalTable: "IPDOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDOrderService_IPDOrder_IPDOrderId",
                table: "IPDOrderService",
                column: "IPDOrderId",
                principalTable: "IPDOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDRound_IPDOrder_IPDOrderID",
                table: "IPDRound",
                column: "IPDOrderID",
                principalTable: "IPDOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
