using System;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class WarehouseItemDTO : WarehouseItem
    {
        public string WarehouseName { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string BatchName { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        [SkipProperty]
        public bool NearExpiry { get; set; }
        [SkipProperty]
        public string QtyString { get; set; }
    }
}
