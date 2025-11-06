using System;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("PackingUnit")]
    public class PackingUnit
    {
        public PackingUnit()
        {
        }

        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public int UnitLevel { get; set; }
        public int QtyInParent { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal SaleAmount { get; set; }
        public bool IsDefault { get; set; }

        [NotMapped]
        [SkipProperty]
        public string UnitName { get; set; }

        [SkipProperty]
        public Item Item { get; set; }
        [SkipProperty]
        public Unit Unit { get; set; }
    }
}
