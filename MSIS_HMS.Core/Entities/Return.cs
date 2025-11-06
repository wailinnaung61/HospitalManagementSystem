using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Return")]
    public class Return:BranchEntity
    {
        public Return()
        {

        }
        public DateTime Date { get; set; }
        public int OutletId { get; set; } // From
        public int WarehouseId { get; set; } // To
        public string Remark { get; set; }

        [NotMapped]
        public string OutletName { get; set; }
        [NotMapped]
        public string WarehouseName { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        [SkipProperty]
        public Outlet Outlet { get; set; }
        [SkipProperty]
        public Warehouse Warehouse { get; set; }
        [SkipProperty]
        public ICollection<ReturnItem> ReturnItems { get; set; }
        
    }
}
