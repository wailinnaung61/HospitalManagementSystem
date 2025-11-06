using System;
using System.Collections.Generic;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class PaymentAmountDTO
    {
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }
        [SkipProperty]
        public decimal Discount { get; set; }
        [SkipProperty]
        public decimal Tax { get; set; }
        [SkipProperty]
        public decimal GrandTotal { get; set; }
    }
}
