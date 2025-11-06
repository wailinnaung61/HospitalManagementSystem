using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddLabResultFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LabResultFile",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabResultId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    AttachmentPath = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabResultFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabResultFile_LabResult_LabResultId",
                        column: x => x.LabResultId,
                        principalTable: "LabResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabResultFile_LabResultId",
                table: "LabResultFile",
                column: "LabResultId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabResultFile");
        }
    }
}
