using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Bed")]
   public class Bed:BranchEntity
    {
        public string No { get; set; }
        public string Status { get; set; }
        public int RoomId { get; set; }
        public int BedTypeId { get; set; }
        public decimal Price { get; set; }
        [NotMapped]
        //[SkipProperty]
        public string BedTypeName { get; set; }
        [NotMapped]
        //[SkipProperty]
        public string RoomNo { get; set; }

        [SkipProperty]
        public Room Room { get; set; }
        [SkipProperty]
        public BedType BedType { get; set; }

    }
}
