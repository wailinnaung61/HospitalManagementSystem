using System;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("WarehouseItem")]
    public class WarehouseItem
    {
        public WarehouseItem()
        {
        }

        public int WarehouseId { get; set; }
        public int ItemId { get; set; }
        public int BatchId { get; set; }
        public int Qty { get; set; }

        [SkipProperty]
        public Warehouse Warehouse { get; set; }
        [SkipProperty]
        public Item Item { get; set; }
        [SkipProperty]
        public Batch Batch { get; set; }
    }
}
