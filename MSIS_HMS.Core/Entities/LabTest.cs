using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("LabTest")]
    public class LabTest : BranchEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsLabReport { get; set; }
        public LabCategoryEnum Category { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ReferralFee { get; set; }
        public FeeTypeEnum ReferralFeeType { get; set; }

        [SkipProperty]
        public ICollection<LabTestDetail> LabTestDetails { get; set; }
        [SkipProperty]
        public ICollection<LabPerson_LabTest> LabPerson_LabTests { get; set; }
    }
}
