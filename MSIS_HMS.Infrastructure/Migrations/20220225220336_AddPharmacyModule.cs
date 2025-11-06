using Microsoft.EntityFrameworkCore.Migrations;

namespace MSIS_HMS.Infrastructure.Migrations
{
    public partial class AddPharmacyModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DisplayName", "DisplayName_mm", "Name" },
                values: new object[] { "Pharmacy", "ဆေးဆိုင်", "Pharmacy Module" });

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DisplayName", "DisplayName_mm", "Name" },
                values: new object[] { "Patient", "လူနာ", "Patient Management Module" });

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DisplayName", "DisplayName_mm", "Name" },
                values: new object[] { "Outpatient Department", "ပြင်ပလူနာဌာန", "Outpatient Department Module" });

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DisplayName", "DisplayName_mm", "Name" },
                values: new object[] { "Inpatient Department", "အတွင်းလူနာဌာန", "Inpatient Department Module" });

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DisplayName", "DisplayName_mm", "Name" },
                values: new object[] { "Operation Theatre", "ခွဲစိတ်ခန်း", "Operation Theatre Module" });

            migrationBuilder.InsertData(
                table: "Module",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Description", "DisplayName", "DisplayName_mm", "IsActive", "IsDelete", "IsMandatory", "ModuleOrder", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 9, "M-9", null, null, "", "Laboratory", "ဓာတ်ခွဲခန်း", true, false, false, 9, "Laboratory Module", null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DisplayName", "DisplayName_mm", "Name" },
                values: new object[] { "Patient", "လူနာ", "Patient Management Module" });

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DisplayName", "DisplayName_mm", "Name" },
                values: new object[] { "Outpatient Department", "ပြင်ပလူနာဌာန", "Outpatient Department Module" });

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DisplayName", "DisplayName_mm", "Name" },
                values: new object[] { "Inpatient Department", "အတွင်းလူနာဌာန", "Inpatient Department Module" });

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DisplayName", "DisplayName_mm", "Name" },
                values: new object[] { "Operation Theatre", "ခွဲစိတ်ခန်း", "Operation Theatre Module" });

            migrationBuilder.UpdateData(
                table: "Module",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DisplayName", "DisplayName_mm", "Name" },
                values: new object[] { "Laboratory", "ဓာတ်ခွဲခန်း", "Laboratory Module" });
        }
    }
}
