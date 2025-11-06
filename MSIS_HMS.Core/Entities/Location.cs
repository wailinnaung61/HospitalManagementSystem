using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Location")]
    public class Location : BranchEntity
    {
        public Location()
        {
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public string Placement { get; set; }
        public int WarehouseId { get; set; }

        [SkipProperty]
        public Warehouse Warehouse { get; set; }
        [SkipProperty]
        public ICollection<ItemLocation> ItemLocations { get; set; }
    }
}
