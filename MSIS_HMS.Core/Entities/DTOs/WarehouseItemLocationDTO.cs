using System;
using System.Collections.Generic;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class WarehouseItemLocationDTO
    {
        public string WarehouseName { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        [SkipProperty]
        public string BatchName { get; set; }
        public int Qty { get; set; }
        public int ExpirationRemindDay { get; set; }
       
    }
}
