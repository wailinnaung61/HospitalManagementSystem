using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddStaffTb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Staff");

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "Staff",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Staff_PositionId",
                table: "Staff",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Position_PositionId",
                table: "Staff",
                column: "PositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staff_Position_PositionId",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Staff_PositionId",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "Staff");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Staff",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
