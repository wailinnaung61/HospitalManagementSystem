using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("WarehouseTransfer")]
    public class WarehouseTransfer : BranchEntity
    {
        public WarehouseTransfer()
        {
        }

        public DateTime Date { get; set; }
        public int FromWarehouseId { get; set; }
        public int ToWarehouseId { get; set; }
        public string Remark { get; set; }
        public int? ToBranchId { get; set; }
        [NotMapped]
        public string FromWarehouseName { get; set; }
        [NotMapped]
        public string ToWarehouseName { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        //[SkipProperty]
        public string ToBranchName { get; set; }
        [SkipProperty]
        public Warehouse FromWarehouse { get; set; }
        [SkipProperty]
        public Warehouse ToWarehouse { get; set; }
        [SkipProperty]
        public ICollection<WarehouseTransferItem> WarehouseTransferItems { get; set; }
        [SkipProperty]
        public ICollection<Branch> ToBranches { get; set; }
    }
}
