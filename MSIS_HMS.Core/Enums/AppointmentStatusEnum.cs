using System;
using System.ComponentModel;

namespace MSIS_HMS.Core.Enums
{
    public enum AppointmentStatusEnum
    {
        [Description("Booked")]
        Booked = 1,
        [Description("InProcess")]
        InProcess = 2,
        [Description("Completed")]
        Completed = 3,
        [Description("Cancel")]
        Cancel = 4
    }
}
