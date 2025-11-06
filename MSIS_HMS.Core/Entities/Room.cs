using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Room")]
    public class Room:BranchEntity
    {
        public string RoomNo { get; set; }
        public int RoomTypeId { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public int WardId { get; set; }
        [NotMapped]
        public string WardName { get; set; }
        [NotMapped]
        public string RoomTypeName { get; set; }
        [SkipProperty]
        public RoomType RoomType { get; set; }
        [SkipProperty]
        public Ward Ward { get; set; }
        [SkipProperty]
        public ICollection<Bed> Beds { get; set; }


    }
}
