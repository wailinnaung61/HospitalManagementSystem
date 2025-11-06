using System;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("OutletItem")]
    public class OutletItem
    {
        public OutletItem()
        {
        }

        public int OutletId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }

        [SkipProperty]
        public Outlet Outlet { get; set; }
        [SkipProperty]
        public Item Item { get; set; }
    }
}
