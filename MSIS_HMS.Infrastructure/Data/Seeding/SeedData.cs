using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;

namespace MSIS_HMS.Infrastructure.Data.Seeding
{
    public static class SeedData
    {
        public static List<Module> preconfirguredModules = new List<Module> {
                new Module() { Id = 1, Name = "User Management And Permission Module", Code = "M-1", DisplayName = "User Management And Permission", DisplayName_mm = "အသုံးပြုသူများနှင့် ခွင့်ပြုချက်များ", Description = "", IsMandatory = true, IsActive = true, IsDelete = false, ModuleOrder = 1 },
                new Module() { Id = 2, Name = "Admin Tools", Code = "M-2", DisplayName = "Admin Tools", DisplayName_mm = "ဆက်တင်များ", Description = "", IsMandatory = true, IsActive = true, IsDelete = false, ModuleOrder = 2 },
                new Module() { Id = 3, Name = "Inventory Module", Code = "M-3", DisplayName = "Inventory", DisplayName_mm = "ပစ္စည်းစာရင်း", Description = "", IsMandatory = true, IsActive = true, IsDelete = false, ModuleOrder = 3 },
                new Module() { Id = 4, Name = "Pharmacy Module", Code = "M-4", DisplayName = "Pharmacy", DisplayName_mm = "ဆေးဆိုင်", Description = "", IsMandatory = false, IsActive = true, IsDelete = false, ModuleOrder = 4 },
                new Module() { Id = 5, Name = "Patient Management Module", Code = "M-5", DisplayName = "Patient", DisplayName_mm = "လူနာ", Description = "", IsMandatory = false, IsActive = true, IsDelete = false, ModuleOrder = 5 },
                new Module() { Id = 6, Name = "Outpatient Department Module", Code = "M-6", DisplayName = "Outpatient Department", DisplayName_mm = "ပြင်ပလူနာဌာန", Description = "", IsMandatory = false, IsActive = true, IsDelete = false, ModuleOrder = 6 },
                new Module() { Id = 7, Name = "Inpatient Department Module", Code = "M-7", DisplayName = "Inpatient Department", DisplayName_mm = "အတွင်းလူနာဌာန", Description = "", IsMandatory = false, IsActive = true, IsDelete = false, ModuleOrder = 7 },
                new Module() { Id = 8, Name = "Operation Theatre Module", Code = "M-8", DisplayName = "Operation Theatre", DisplayName_mm = "ခွဲစိတ်ခန်း", Description = "", IsMandatory = false, IsActive = true, IsDelete = false, ModuleOrder = 8 },
                new Module() { Id = 9, Name = "Laboratory Module", Code = "M-9", DisplayName = "Laboratory", DisplayName_mm = "ဓာတ်ခွဲခန်း", Description = "", IsMandatory = false, IsActive = true, IsDelete = false, ModuleOrder = 9 },
            };
    }
}
