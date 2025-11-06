using System;
using System.Collections.Generic;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class PaymentDetailDTO
    {
        public DateTime Date { get; set; }
        public int Day { get; set; }
        public decimal RoomCharges { get; set; }
        public decimal Medications { get; set; }
        public decimal Services { get; set; }
        public decimal Fees { get; set; }
        public decimal Food { get; set; }
        [SkipProperty]
        public decimal Total { get; set; }
    }
}
