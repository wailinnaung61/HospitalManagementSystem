using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Scratch")]
    public class Scratch : BranchEntity
    {
        public Scratch()
        {
        }
        public DateTime Date { get; set; }
        public int WarehouseId { get; set; }
        public string Remark { get; set; }
        [NotMapped]
        public string WarehouseName { get; set; }
        [SkipProperty]
        public Warehouse Warehouse { get; set; }
        [SkipProperty]
        public ICollection<ScratchItem> ScratchItems { get; set; }
    }
}
