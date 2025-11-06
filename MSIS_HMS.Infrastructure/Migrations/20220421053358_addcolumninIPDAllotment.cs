using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class addcolumninIPDAllotment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPDAllotment_Bed_BedId",
                table: "IPDAllotment");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDAllotment_Room_RoomId",
                table: "IPDAllotment");

            migrationBuilder.DropIndex(
                name: "IX_IPDAllotment_BedId",
                table: "IPDAllotment");

            migrationBuilder.DropIndex(
                name: "IX_IPDAllotment_RoomId",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "BedId",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "IPDAllotment");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IPDAllotment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "IPDAllotment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FromBedId",
                table: "IPDAllotment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FromRoomId",
                table: "IPDAllotment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "IPDAllotment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "IPDAllotment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToBedId",
                table: "IPDAllotment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToRoomId",
                table: "IPDAllotment",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IPDAllotment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "IPDAllotment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IPDAllotment_FromRoomId",
                table: "IPDAllotment",
                column: "FromRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_IPDAllotment_Room_FromRoomId",
                table: "IPDAllotment",
                column: "FromRoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPDAllotment_Room_FromRoomId",
                table: "IPDAllotment");

            migrationBuilder.DropIndex(
                name: "IX_IPDAllotment_FromRoomId",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "FromBedId",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "FromRoomId",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "ToBedId",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "ToRoomId",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "IPDAllotment");

            migrationBuilder.AddColumn<int>(
                name: "BedId",
                table: "IPDAllotment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "IPDAllotment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IPDAllotment_BedId",
                table: "IPDAllotment",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_IPDAllotment_RoomId",
                table: "IPDAllotment",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_IPDAllotment_Bed_BedId",
                table: "IPDAllotment",
                column: "BedId",
                principalTable: "Bed",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDAllotment_Room_RoomId",
                table: "IPDAllotment",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
