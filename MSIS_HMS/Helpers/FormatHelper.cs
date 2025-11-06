using System;
namespace MSIS_HMS.Helpers
{
    public static class FormatHelper
    {
        public static string FormatMoney<T>(this T amount)
        {
            return Convert.ToDecimal(amount).ToString("N");
        }
    }
}
