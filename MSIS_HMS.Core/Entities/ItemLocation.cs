using System;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("ItemLocation")]
    public class ItemLocation : BranchEntity
    {
        public ItemLocation()
        {
        }

        public int ItemId { get; set; }
        public int LocationId { get; set; }
        public string Remark { get; set; }
        public int WarehouseId { get; set; }
        public int BatchId { get; set; }
        [NotMapped]
        public string ItemName { get; set; }
        [NotMapped]
        public string LocationName { get; set; }
        [NotMapped]
        public string BatchName { get; set; }
        [NotMapped]
        public string WarehouseName { get; set; }
        [SkipProperty]
        public Item Item { get; set; }
        [SkipProperty]
        public Location Location { get; set; }
        [SkipProperty]
        public Warehouse Warehouse { get; set; }
        [SkipProperty]
        public Batch Batch { get; set; }
    }
}
