using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
   public class IPDRecordDetailReportDTO
    {
        public int No { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public string Qty { get; set; }
        public string UnitName { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }
       
    }
}
