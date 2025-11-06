using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddOutletFKToWard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Ward_OutletId",
                table: "Ward",
                column: "OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ward_Outlet_OutletId",
                table: "Ward",
                column: "OutletId",
                principalTable: "Outlet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ward_Outlet_OutletId",
                table: "Ward");

            migrationBuilder.DropIndex(
                name: "IX_Ward_OutletId",
                table: "Ward");
        }
    }
}
