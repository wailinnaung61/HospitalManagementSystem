using System;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("OrderService")]
    public class OrderService
    {
        public OrderService()
        {
        }

        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ServiceId { get; set; }
        public int? ReferrerId { get; set; }
        public FeeTypeEnum FeeType { get; set; }
        public decimal Fee { get; set; }
        public decimal ReferralFee { get; set; } // Calculated with Service charges, Fee Type and Fee
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
        public bool IsFOC { get; set; } // FOC
        public int SortOrder { get; set; }

        [NotMapped]
        public string ReferrerName { get; set; }
        [NotMapped]
        public string ServiceName { get; set; }

        [SkipProperty]
        public Order Order { get; set; }
        [SkipProperty]
        public Service Service { get; set; }
        [SkipProperty]
        public Referrer Referrer { get; set; }
    }
}
