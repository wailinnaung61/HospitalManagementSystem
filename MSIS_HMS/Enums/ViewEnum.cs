using System;
using System.ComponentModel;

namespace MSIS_HMS.Enums
{
    public class ViewEnum
    {
        public enum Action
        {
            [Description("Create")]
            Create = 1,
            [Description("Edit")]
            Edit = 2,
            [Description("Delete")]
            Delete = 3,         
        }
    }
}
