using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddFromTimeToTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<string>(
                name: "FromTime",
                table: "Appointment",
                nullable: true);

           
            migrationBuilder.AddColumn<string>(
                name: "ToTime",
                table: "Appointment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "FromTime",
                table: "Appointment");
            
            migrationBuilder.DropColumn(
                name: "ToTime",
                table: "Appointment");

           
        }
    }
}
