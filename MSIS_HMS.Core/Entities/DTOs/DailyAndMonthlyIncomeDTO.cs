using System;
using System.Collections.Generic;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class DailyAndMonthlyIncomeDTO
    {
       [SkipProperty]
       public int No { get; set; }
        public string Module { get; set; }
        public decimal DailyIncome { get; set; }
        public decimal MonthlyIncome { get; set; }
    }
}
