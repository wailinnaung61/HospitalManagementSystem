using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class changefktoToRoomInIPDAllotment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPDAllotment_Room_FromRoomId",
                table: "IPDAllotment");

            migrationBuilder.DropIndex(
                name: "IX_IPDAllotment_FromRoomId",
                table: "IPDAllotment");

            migrationBuilder.AlterColumn<int>(
                name: "ToRoomId",
                table: "IPDAllotment",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FromRoomId",
                table: "IPDAllotment",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_IPDAllotment_ToRoomId",
                table: "IPDAllotment",
                column: "ToRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_IPDAllotment_Room_ToRoomId",
                table: "IPDAllotment",
                column: "ToRoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPDAllotment_Room_ToRoomId",
                table: "IPDAllotment");

            migrationBuilder.DropIndex(
                name: "IX_IPDAllotment_ToRoomId",
                table: "IPDAllotment");

            migrationBuilder.AlterColumn<int>(
                name: "ToRoomId",
                table: "IPDAllotment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "FromRoomId",
                table: "IPDAllotment",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IPDAllotment_FromRoomId",
                table: "IPDAllotment",
                column: "FromRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_IPDAllotment_Room_FromRoomId",
                table: "IPDAllotment",
                column: "FromRoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
