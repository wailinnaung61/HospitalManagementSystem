using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Warehouse")]
    public class Warehouse : BranchEntity
    {
        public Warehouse()
        {
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public string Location { get; set; }

        [SkipProperty]
        public ICollection<Location> Locations { get; set; }
        [SkipProperty]
        public ICollection<Outlet> Outlets { get; set; }
        [SkipProperty]
        public ICollection<WarehouseItem> WarehouseItems { get; set; }
        [SkipProperty]
        public ICollection<WarehouseTransfer> FromWarehouseTransfers { get; set; }
        [SkipProperty]
        public ICollection<WarehouseTransfer> ToWarehouseTransfers { get; set; }
        [SkipProperty]
        public ICollection<OutletTransfer> OutletTransfers { get; set; }
        [SkipProperty]
        public ICollection<DeliverOrder> DeliverOrders { get; set; }
    }
}
