using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class UpdateIPDRecordIDToIPDRecordIdInIPDRoundAndIPDStaff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPDRound_IPDRecord_IPDRecordID",
                table: "IPDRound");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDStaff_IPDOrder_IPDOrderID",
                table: "IPDStaff");

            migrationBuilder.RenameColumn(
                name: "IPDOrderID",
                table: "IPDStaff",
                newName: "IPDOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_IPDStaff_IPDOrderID",
                table: "IPDStaff",
                newName: "IX_IPDStaff_IPDOrderId");

            migrationBuilder.RenameColumn(
                name: "IPDRecordID",
                table: "IPDRound",
                newName: "IPDRecordId");

            migrationBuilder.RenameIndex(
                name: "IX_IPDRound_IPDRecordID",
                table: "IPDRound",
                newName: "IX_IPDRound_IPDRecordId");

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderId",
                table: "IPDStaff",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "IPDRecordId",
                table: "IPDStaff",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IPDStaff_IPDRecordId",
                table: "IPDStaff",
                column: "IPDRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_IPDRound_IPDRecord_IPDRecordId",
                table: "IPDRound",
                column: "IPDRecordId",
                principalTable: "IPDRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDStaff_IPDRecord_IPDRecordId",
                table: "IPDStaff",
                column: "IPDRecordId",
                principalTable: "IPDRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPDRound_IPDRecord_IPDRecordId",
                table: "IPDRound");

            migrationBuilder.DropForeignKey(
                name: "FK_IPDStaff_IPDRecord_IPDRecordId",
                table: "IPDStaff");

            migrationBuilder.DropIndex(
                name: "IX_IPDStaff_IPDRecordId",
                table: "IPDStaff");

            migrationBuilder.DropColumn(
                name: "IPDRecordId",
                table: "IPDStaff");

            migrationBuilder.RenameColumn(
                name: "IPDOrderId",
                table: "IPDStaff",
                newName: "IPDOrderID");

            migrationBuilder.RenameIndex(
                name: "IX_IPDStaff_IPDOrderId",
                table: "IPDStaff",
                newName: "IX_IPDStaff_IPDOrderID");

            migrationBuilder.RenameColumn(
                name: "IPDRecordId",
                table: "IPDRound",
                newName: "IPDRecordID");

            migrationBuilder.RenameIndex(
                name: "IX_IPDRound_IPDRecordId",
                table: "IPDRound",
                newName: "IX_IPDRound_IPDRecordID");

            migrationBuilder.AlterColumn<int>(
                name: "IPDOrderID",
                table: "IPDStaff",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDRound_IPDRecord_IPDRecordID",
                table: "IPDRound",
                column: "IPDRecordID",
                principalTable: "IPDRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IPDStaff_IPDOrder_IPDOrderID",
                table: "IPDStaff",
                column: "IPDOrderID",
                principalTable: "IPDOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
