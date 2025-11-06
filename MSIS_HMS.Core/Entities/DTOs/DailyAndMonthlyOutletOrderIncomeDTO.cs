using System;
using System.Collections.Generic;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class DailyAndMonthlyOutletOrderIncomeDTO
    {
        [SkipProperty]
        public int No { get; set; }
        public int OutletId { get; set; }
        public string OutletName { get; set; }
        public decimal DailyIncome { get; set; }
        [SkipProperty]
        public int DailyOrder { get; set; }
        public decimal MonthlyIncome { get; set; }
    }
}
