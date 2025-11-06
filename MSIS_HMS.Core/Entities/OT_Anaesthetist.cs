using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("OT_Anaesthetist")]
    public class OT_Anaesthetist: Entity
    {
        public OT_Anaesthetist()
        {

        }
        public int DoctorId { get; set; }
        public OTDoctorTypeEnum OTDoctorTypeEnum { get; set; }
        public decimal Fee { get; set; }
        public int OperationTreaterId { get; set; }
        public int SortOrder { get; set; }

        [NotMapped]
        public string DoctorName { get; set; }
        [SkipProperty]
        public Doctor Doctor { get; set; }
        [SkipProperty]
        public OperationTreater OperationTreater { get; set; }
    }
}
