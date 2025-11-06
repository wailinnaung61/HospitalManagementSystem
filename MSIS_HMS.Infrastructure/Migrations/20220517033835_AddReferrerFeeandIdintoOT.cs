using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddReferrerFeeandIdintoOT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ReferrerFee",
                table: "OperationTreater",
                nullable: true);


            migrationBuilder.AddColumn<int>(
                name: "ReferrerId",
                table: "OperationTreater",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationTreater_ReferrerId",
                table: "OperationTreater",
                column: "ReferrerId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationTreater_Referrer_ReferrerId",
                table: "OperationTreater",
                column: "ReferrerId",
                principalTable: "Referrer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationTreater_Referrer_ReferrerId",
                table: "OperationTreater");

            migrationBuilder.DropIndex(
                name: "IX_OperationTreater_ReferrerId",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "ReferrerFee",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "ReferrerId",
                table: "OperationTreater");
        }
    }
}
