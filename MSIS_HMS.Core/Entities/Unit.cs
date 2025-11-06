using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Unit")]
    public class Unit : BranchEntity
    {
        public Unit()
        {
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string ShortForm { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public int UnitLevel { get; set; }

        [SkipProperty]
        public ICollection<PackingUnit> PackingUnits { get; set; }
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
