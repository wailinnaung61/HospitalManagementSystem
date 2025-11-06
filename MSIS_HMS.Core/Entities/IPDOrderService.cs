using System;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("IPDOrderService")]
    public class IPDOrderService:Entity
    {
        public IPDOrderService()
        {
        }
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int IPDRecordId { get; set; }
        public int ServiceId { get; set; }
        public FeeTypeEnum FeeType { get; set; }
        public decimal Fee { get; set; }
        public decimal ReferralFee { get; set; } // Calculated with Service charges, Fee Type and Fee
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
        public bool IsFOC { get; set; }
        public int SortOrder { get; set; } 
        [NotMapped]
        public string ServiceName { get; set; }

        [SkipProperty]
        public IPDRecord IPDRecord { get; set; }
        [SkipProperty]
        public Service Service { get; set; }
       
    }
}
