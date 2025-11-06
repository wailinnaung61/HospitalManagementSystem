using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("IPDPayment")]
    public class IPDPayment:Entity
    {
        public decimal Amount { get; set; }
        public PaymentTypeEnum PaymentType { get; set; }
        public DateTime Date { get; set; }
        public string PaidBy { get; set; }
        public int IPDRecordId { get; set; }
        [SkipProperty]
        public IPDRecord IPDRecord { get; set; }
    }
}
