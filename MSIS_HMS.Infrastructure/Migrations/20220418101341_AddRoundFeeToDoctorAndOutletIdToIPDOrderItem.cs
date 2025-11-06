using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddRoundFeeToDoctorAndOutletIdToIPDOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutletId",
                table: "IPDOrderItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RoundFee",
                table: "Doctor",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IPDOrderItem_OutletId",
                table: "IPDOrderItem",
                column: "OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_IPDOrderItem_Outlet_OutletId",
                table: "IPDOrderItem",
                column: "OutletId",
                principalTable: "Outlet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPDOrderItem_Outlet_OutletId",
                table: "IPDOrderItem");

            migrationBuilder.DropIndex(
                name: "IX_IPDOrderItem_OutletId",
                table: "IPDOrderItem");

            migrationBuilder.DropColumn(
                name: "OutletId",
                table: "IPDOrderItem");

            migrationBuilder.DropColumn(
                name: "RoundFee",
                table: "Doctor");
        }
    }
}
