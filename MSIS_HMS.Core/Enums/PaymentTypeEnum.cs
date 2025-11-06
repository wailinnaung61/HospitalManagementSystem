using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MSIS_HMS.Core.Enums
{
    public enum PaymentTypeEnum
    {
        [Description("Cash")]
        Cash = 1,
        [Description("Credit")]
        Credit = 2,
        [Description("Advance")]
        Advance = 3,
       
    }
}
