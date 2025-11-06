using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddFindingsAndPostOpTreatmentintoOperationTreater : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "SentToPathology",
                table: "OperationTreater",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Findings",
                table: "OperationTreater",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostOpTreatment",
                table: "OperationTreater",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Findings",
                table: "OperationTreater");

            migrationBuilder.DropColumn(
                name: "PostOpTreatment",
                table: "OperationTreater");

            migrationBuilder.AlterColumn<bool>(
                name: "SentToPathology",
                table: "OperationTreater",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
