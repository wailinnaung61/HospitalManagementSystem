using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddTimestampToAllTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Nurse",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Nurse",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Nurse",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Nurse",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Logs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Logs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Logs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Logs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Department",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Department",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Department",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Department",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Branch",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Branch",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Nurse");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Nurse");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Nurse");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Nurse");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Branch");
        }
    }
}
