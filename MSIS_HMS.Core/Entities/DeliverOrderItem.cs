using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("DeliverOrderItem")]
    public class DeliverOrderItem
    {
        public DeliverOrderItem()
        {
        }

        public int Id { get; set; }
        public int DeliverOrderId { get; set; }
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public int BatchId { get; set; }

        public decimal UnitPrice { get; set; }
        [Required]
        public string UnitName { get; set; }
        public int Qty { get; set; }
        public int QtyInSmallestUnit { get; set; }
        public bool IsGiftItem { get; set; } // FOC
        public int SortOrder { get; set; }

        [NotMapped]
        [SkipProperty]
        public string ItemName { get; set; }
        [NotMapped]
        [SkipProperty]
        public string BatchName { get; set; }

        [SkipProperty]
        public DeliverOrder DeliverOrder { get; set; }
        [SkipProperty]
        public Item Item { get; set; }
        [SkipProperty]
        public Unit Unit { get; set; }
        [SkipProperty]
        public Batch Batch { get; set; }
    }
}
