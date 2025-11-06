using MSIS_HMS.Core.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("IPDOrderItem")]
    public class IPDOrderItem:Entity
    {
        public IPDOrderItem()
        {
        }
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int IPDRecordId { get; set; }
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
        public int QtyInSmallestUnit { get; set; }
        public bool IsFOC { get; set; }
        public int OutletId { get; set; }
        public int SortOrder { get; set; }

        [NotMapped]
        public string UnitName { get; set; }
        [NotMapped]
        public string ItemName { get; set; }
        [NotMapped]
        public string ShortForm { get; set; }
        [NotMapped]
        public string OutletName { get; set; }

        [SkipProperty]
        public IPDRecord IPDRecord { get; set; }
        [SkipProperty]
        public Item Item { get; set; }
        [SkipProperty]
        public Unit Unit { get; set; }
        [SkipProperty]
        public Outlet Outlet { get; set; }
    }

}
