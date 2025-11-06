using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class SaleItemDTO
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int Qty { get; set; }
        public decimal Amount { get; set; }
        public string OutletName { get; set; }
      
        [NotMapped]
        [SkipProperty]
        public string QtyString { get; set; }
        [NotMapped]
        [SkipProperty]
        public int No { get; set; }
    }
}
