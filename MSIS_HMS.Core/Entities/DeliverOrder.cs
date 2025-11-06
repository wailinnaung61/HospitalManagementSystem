using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("DeliverOrder")]
    public class DeliverOrder : BranchEntity
    {
        public DeliverOrder()
        {
        }

        public string VoucherNo { get; set; }
        public DateTime Date { get; set; }
        public string Customer { get; set; }
        public int WarehouseId { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Remark { get; set; }
        [NotMapped]
        public string WarehouseName { get; set; }
        [SkipProperty]
        public Warehouse Warehouse { get; set; }
        [SkipProperty]
        public ICollection<DeliverOrderItem> DeliverOrderItems { get; set; }
    }
}
