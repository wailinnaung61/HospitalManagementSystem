using System;
using System.ComponentModel;

namespace MSIS_HMS.Core.Enums
{
    public enum DepartmentTypeEnum : int
    {
        [Description("Out Patient Department")]
        OPD = 1,
        [Description("In Patient Department")]
        IPD = 2,
        [Description("Operation Theatre")]
        OT = 3
    }
}
