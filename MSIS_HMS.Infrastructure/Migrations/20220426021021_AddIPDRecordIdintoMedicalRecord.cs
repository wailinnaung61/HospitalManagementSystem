using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddIPDRecordIdintoMedicalRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IPDRecordId",
                table: "MedicalRecord",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IPDStaff",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "IPDStaff",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "IPDStaff",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IPDStaff",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "IPDStaff",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IPDRound",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "IPDRound",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "IPDRound",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IPDRound",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "IPDRound",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IPDOrderService",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "IPDOrderService",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "IPDOrderService",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IPDOrderService",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "IPDOrderService",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IPDOrderItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "IPDOrderItem",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "IPDOrderItem",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IPDOrderItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "IPDOrderItem",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IPDFood",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "IPDFood",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "IPDFood",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IPDFood",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "IPDFood",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IPDRecordId",
                table: "MedicalRecord");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IPDStaff");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "IPDStaff");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "IPDStaff");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IPDStaff");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "IPDStaff");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IPDRound");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "IPDRound");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "IPDRound");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IPDRound");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "IPDRound");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IPDOrderService");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "IPDOrderService");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "IPDOrderService");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IPDOrderService");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "IPDOrderService");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IPDOrderItem");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "IPDOrderItem");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "IPDOrderItem");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IPDOrderItem");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "IPDOrderItem");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IPDFood");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "IPDFood");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "IPDFood");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IPDFood");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "IPDFood");
        }
    }
}
