using System;
using MSIS_HMS.Core.Enums;

namespace MSIS_HMS.Infrastructure.Helpers
{
    public static class CalculationExtensions
    {
        public static decimal GetTotal()
        {
            return 0M;
        }

        public static decimal GetFee(decimal UnitPrice, decimal Fee, FeeTypeEnum FeeType)
        {
            var _fee = 0M;
            switch(FeeType)
            {
                case FeeTypeEnum.Percentage: _fee = (UnitPrice / 100) * Fee; break;
                case FeeTypeEnum.FixedAmount: _fee = Fee; break;
                default: break;
            }
            return _fee;
        }
    }
}
