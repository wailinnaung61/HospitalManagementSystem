using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MSIS_HMS.Core.Enums
{
    public enum IPDStatusEnum
    {
        [Description("DisCharged")]
        Discharged = 1,
        [Description("Draft")]
        Draft=2,
        [Description("Admission")]
        Admission=3
    }
}
