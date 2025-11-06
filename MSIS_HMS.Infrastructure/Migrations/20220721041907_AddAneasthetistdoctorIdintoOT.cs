using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddAneasthetistdoctorIdintoOT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AneasthetistDoctorId",
                table: "OperationTreater",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AneasthetistFee",
                table: "OperationTreater",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AneasthetistDoctorId",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "AneasthetistFee",
                table: "OperationTreater");
        }
    }
}
