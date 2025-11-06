using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("OT_Staff")]
    public class OT_Staff:Entity
    {
        public OT_Staff()
        {

        }
        public int StaffId { get; set; }
        public OTStaffTypeEnum OTStaffTypeEnum { get; set; }
        public decimal Fee { get; set; }
        public int OperationTreaterId { get; set; }
        public int SortOrder { get; set; }
        [NotMapped]
        public string StaffName { get; set; }
        [SkipProperty]
        public Staff Staff { get; set; }
        [SkipProperty]
        public OperationTreater OperationTreater { get; set; }

    }
}
