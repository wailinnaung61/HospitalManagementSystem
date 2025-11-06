using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MSIS_HMS.Core.Enums
{
    public enum OTDoctorTypeEnum
    {
        [Description("First Assistant")]
        [Display(Name="First Assistant")]
        First_Assistant = 1,
        [Description("Second Assistant")]
        [Display(Name = "Second Assistant")]
        Second_Assistant = 2,
        //[Description("Anaesthetist")]
        //Anaesthetist=3

    }
}
