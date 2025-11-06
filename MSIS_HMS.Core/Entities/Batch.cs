using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Batch")]
    public class Batch : BranchEntity
    {
        public Batch()
        {
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public string BatchNumber { get; set; }
        public int ItemId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Description { get; set; }
        [NotMapped]
        //[SkipProperty]
        public string ItemName { get; set; }
        [SkipProperty]
        public Item Item { get; set; }
        [SkipProperty]
        public ICollection<WarehouseItem> WarehouseItems { get; set; }
        [SkipProperty]
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
        [SkipProperty]
        public ICollection<WarehouseTransferItem> WarehouseTransferItems { get; set; }
        [SkipProperty]
        public ICollection<OutletTransferItem> OutletTransferItems { get; set; }
        [SkipProperty]
        public ICollection<DeliverOrderItem> DeliverOrderItems { get; set; }
    }
}
