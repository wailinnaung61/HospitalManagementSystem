using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
   public class DailyIncomeDTO
    {
        public decimal OrderIncome { get; set; }
        public decimal LabOrderIncome { get; set; }
        public decimal IPDIncome { get; set; }
        public decimal OTIncome { get; set; }
        public decimal OPDIncome { get; set; }
        
    }
}
