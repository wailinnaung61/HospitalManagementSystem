using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class FoodDTO
    {
        public int No { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }
        public int BranchId { get; set; }
    }
}
