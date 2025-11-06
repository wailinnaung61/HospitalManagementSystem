using System;
using System.ComponentModel;

namespace MSIS_HMS.Core.Enums
{
    public enum LabEnum
    {
    }

    public enum LabPersonTypeEnum
    {
        [Description("Technician")]
        Technician = 1,
        [Description("Consultant")]
        Consultant = 2
    }

    public enum LabCategoryEnum
    {
        [Description("Laboratory")]
        Laboratory = 1,
        [Description("Radiology")]
        Radiology = 2
    }

    public enum LabResultType
    {
        [Description("Text")]
        NoResult = 0,
        [Description("Range")]
        Range = 1,
        [Description("Select")]
        Select = 2
    }
}
