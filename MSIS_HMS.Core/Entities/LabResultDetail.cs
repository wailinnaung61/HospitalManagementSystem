using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("LabResultDetail")]
    public class LabResultDetail
    {
        public LabResultDetail()
        {
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int LabResultId { get; set; }
        public string LabUnit { get; set; }
        public string Result { get; set; }
        public double? MinRange { get; set; }
        public double? MaxRange { get; set; }
        public string SelectList { get; set; }
        public LabResultType LabResultType { get; set; }
        public bool IsTitle { get; set; }
        public bool IsPrinted { get; set; }
        public string Remark { get; set; }
        public int SortOrder { get; set; }
        public int TestId { get; set; }
        public int? TechnicianId { get; set; }
        public decimal TechnicianFee { get; set; }
        public FeeTypeEnum TechnicianFeeType { get; set; }

        public int? ConsultantId { get; set; }
        public decimal ConsultantFee { get; set; }
        public FeeTypeEnum ConsultantFeeType { get; set; }


        [NotMapped]
        public string LabResultNo { get; set; }

        [SkipProperty]
        public LabResult LabResult { get; set; }
        [SkipProperty]
        public LabTest LabTest { get; set; }
        [SkipProperty]
        public LabPerson Technician { get; set; }
        [SkipProperty]
        public LabPerson Consultant { get; set; }
    }
}
