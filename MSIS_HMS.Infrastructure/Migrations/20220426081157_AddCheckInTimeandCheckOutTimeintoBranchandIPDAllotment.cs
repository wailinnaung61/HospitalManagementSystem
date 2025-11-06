using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddCheckInTimeandCheckOutTimeintoBranchandIPDAllotment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "CheckInTime",
                table: "IPDAllotment",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CheckOutTime",
                table: "IPDAllotment",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CheckInTime",
                table: "Branch",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CheckOutTime",
                table: "Branch",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "CheckOutTime",
                table: "IPDAllotment");

            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "CheckOutTime",
                table: "Branch");
        }
    }
}
