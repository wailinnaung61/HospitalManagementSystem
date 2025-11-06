using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddLabPerson_LabTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LabPerson_LabTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabPersonId = table.Column<int>(nullable: false),
                    LabTestId = table.Column<int>(nullable: false),
                    Fee = table.Column<decimal>(nullable: false),
                    FeeType = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabPerson_LabTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabPerson_LabTest_LabPerson_LabPersonId",
                        column: x => x.LabPersonId,
                        principalTable: "LabPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabPerson_LabTest_LabTest_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabPerson_LabTest_LabPersonId",
                table: "LabPerson_LabTest",
                column: "LabPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_LabPerson_LabTest_LabTestId",
                table: "LabPerson_LabTest",
                column: "LabTestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabPerson_LabTest");
        }
    }
}
