using System;
using System.ComponentModel;

namespace MSIS_HMS.Infrastructure.Enums
{
    public class RoleEnum
    {
        public enum Role
        {
            [Description("Superadmin")]
            Superadmin,
            [Description("Owner")]
            Owner,
            [Description("Admin")]
            Admin,
            [Description("Warehouse")]
            Warehouse,
            [Description("Reception")]
            Reception,
            [Description("Pharmacy")]
            Pharmacy,
            [Description("Cashier")]
            Cashier,
            [Description("Doctor")]
            Doctor,
            [Description("Lab")]
            Lab,
        }
    }
}
