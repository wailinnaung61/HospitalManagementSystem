using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class SeedModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                table: "Module",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Module",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Description", "DisplayName", "DisplayName_mm", "IsActive", "IsDelete", "IsMandatory", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "M-1", null, null, "", "User Management And Permission", "အသုံးပြုသူများနှင့် ခွင့်ပြုချက်များ", true, false, true, "User Management And Permission Module", null, null },
                    { 2, "M-2", null, null, "", "Admin Tools", "ဆက်တင်များ", true, false, true, "Admin Tools", null, null },
                    { 3, "M-3", null, null, "", "Inventory", "ပစ္စည်းစာရင်း", true, false, true, "Inventory Module", null, null },
                    { 4, "M-4", null, null, "", "Patient", "လူနာ", true, false, false, "Patient Management Module", null, null },
                    { 5, "M-5", null, null, "", "Outpatient Department", "ပြင်ပလူနာဌာန", true, false, false, "Outpatient Department Module", null, null },
                    { 6, "M-6", null, null, "", "Inpatient Department", "အတွင်းလူနာဌာန", true, false, false, "Inpatient Department Module", null, null },
                    { 7, "M-7", null, null, "", "Operation Theatre", "ခွဲစိတ်ခန်း", true, false, false, "Operation Theatre Module", null, null },
                    { 8, "M-8", null, null, "", "Laboratory", "ဓာတ်ခွဲခန်း", true, false, false, "Laboratory Module", null, null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "IsMandatory",
                table: "Module");
        }
    }
}
