using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("OperationRoom")]
    public class OperationRoom : BranchEntity
    {
        public string RoomNo { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public int WardId { get; set; }
        [NotMapped]
        //[SkipProperty]
        public string WardName { get; set; }

        [SkipProperty]
        public Ward Ward { get; set; }
        


    }
}
