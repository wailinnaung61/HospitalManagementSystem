using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class MakeReferrerIdNullableInPatient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patient_Referrer_ReferrerId",
                table: "Patient");

            migrationBuilder.AlterColumn<int>(
                name: "ReferrerId",
                table: "Patient",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_Referrer_ReferrerId",
                table: "Patient",
                column: "ReferrerId",
                principalTable: "Referrer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patient_Referrer_ReferrerId",
                table: "Patient");

            migrationBuilder.AlterColumn<int>(
                name: "ReferrerId",
                table: "Patient",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_Referrer_ReferrerId",
                table: "Patient",
                column: "ReferrerId",
                principalTable: "Referrer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
