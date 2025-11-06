using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MSIS_HMS.Core.Enums
{
    public enum OTStaffTypeEnum
    {
        [Description("Scrub Nurse")]
        [Display(Name="Scrub Nurse")]
        Scrub_Nurse=1,
        [Description("Circulation Nurse")]
        [Display(Name = "Circulation Nurse")]
        Circulation = 2,
        [Description("ORA")]
        [Display(Name = "ORA")]
        ORA = 3
    }
}
