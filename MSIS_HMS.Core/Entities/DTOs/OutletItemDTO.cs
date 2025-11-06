using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class OutletItemDTO : OutletItem
    {
        public string OutletName { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
 
        //public int WarehouseId { get; set; }
        //public string WarehouseName { get; set; }
    }
}
