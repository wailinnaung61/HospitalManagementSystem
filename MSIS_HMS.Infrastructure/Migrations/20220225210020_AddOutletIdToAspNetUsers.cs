using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddOutletIdToAspNetUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutletId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OutletId",
                table: "AspNetUsers",
                column: "OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Outlet_OutletId",
                table: "AspNetUsers",
                column: "OutletId",
                principalTable: "Outlet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Outlet_OutletId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OutletId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OutletId",
                table: "AspNetUsers");
        }
    }
}
