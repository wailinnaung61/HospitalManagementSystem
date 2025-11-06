using System;
using System.ComponentModel;

namespace MSIS_HMS.Enums
{
    public static class StatusEnum
    {
        public enum NoticeStatus
        {
            [Description("Save Successfully")]
            Success = 1,
            [Description("Update Successfully")]
            Edit =2,
            [Description("Delete Successfully")]
            Delete = 3,
            Fail = 4,

        }
    }
}
