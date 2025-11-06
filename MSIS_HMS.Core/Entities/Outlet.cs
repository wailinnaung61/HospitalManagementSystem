using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Outlet")]
    public class Outlet : BranchEntity
    {
        public Outlet()
        {
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public string Location { get; set; }
        public int WarehouseId { get; set; }
        [NotMapped]
        public string WarehouseName { get; set; }
        [SkipProperty]
        public Warehouse Warehouse { get; set; }
        [SkipProperty]
        public ICollection<OutletItem> OutletItems { get; set; }
        [SkipProperty]
        public ICollection<OutletTransfer> OutletTransfers { get; set; }
    }
}
