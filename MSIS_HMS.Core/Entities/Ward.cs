using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Ward")]
    public class Ward : Entity
    {
        public int No { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public int FloorId { get; set; }
        public int OutletId { get; set; } // Nearby Outlet

        [NotMapped]
        //[SkipProperty]
        public string DepartmentName { get; set; }
        [NotMapped]
        [SkipProperty]
        public string FloorName { get; set; }
        [NotMapped]
        //[SkipProperty]
        public string OutletName { get; set; }

        [SkipProperty]
        public Department Department { get; set; }
        [SkipProperty]
        public Floor Floor { get; set; }
        [SkipProperty]
        public Outlet Outlet { get; set; }
        [SkipProperty]
        public ICollection<Room> Rooms { get; set; }

    }
}

