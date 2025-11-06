using System;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("LabPerson_LabTest")]
    public class LabPerson_LabTest
    {
        public LabPerson_LabTest()
        {
        }

        public int Id { get; set; }
        public int LabPersonId { get; set; }
        public int LabTestId { get; set; }
        public decimal Fee { get; set; }
        public FeeTypeEnum FeeType { get; set; }
        public int SortOrder { get; set; }

        [NotMapped]
        public string LabPersonName { get; set; }
        [NotMapped]
        public string LabTestName { get; set; }
        [NotMapped]
        public bool IsLabReport { get; set; }
        [NotMapped]
        public LabPersonTypeEnum LabPersonType { get; set; }

        [SkipProperty]
        public LabPerson LabPerson { get; set; }
        [SkipProperty]
        public LabTest LabTest { get; set; }
    }
}
