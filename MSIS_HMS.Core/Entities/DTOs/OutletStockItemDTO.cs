using System;
using System.Collections.Generic;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.DTOs
{
   public class OutletStockItemDTO:OutletItem
    {
        public string OutletName { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        [SkipProperty]
        public string QtyString { get; set; }
    }
}
