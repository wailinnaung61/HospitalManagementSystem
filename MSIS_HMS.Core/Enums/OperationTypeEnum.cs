using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MSIS_HMS.Core.Enums
{
    public enum OperationTypeEnum : int
    {
        [Description("Urgent")]
        [Display(Name = "Urgent")]
        Urgent = 1,
        [Description("Elective")]
        [Display(Name = "Elective")]
        Elective = 2
       
    }
}
