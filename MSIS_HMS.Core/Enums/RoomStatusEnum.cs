using System.ComponentModel;

namespace MSIS_HMS.Core.Enums
{
    public enum RoomStatusEnum
    {
        [Description("Available")]
        Busy = 1,
        [Description("Not Available")]
        NotBusy = 2,
    }
}
