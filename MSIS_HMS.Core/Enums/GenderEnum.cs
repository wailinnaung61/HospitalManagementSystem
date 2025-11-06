using System;
using System.ComponentModel;

namespace MSIS_HMS.Core.Enums
{
    public class GenderEnum
    {
        public enum Gender : int
        {
            [Description("Male")]
            Male = 1,
            [Description("Female")]
            Female = 2,
            [Description("Other")]
            Other = 3
        }
    }
}
