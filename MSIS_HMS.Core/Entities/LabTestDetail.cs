using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("LabTestDetail")]
    public class LabTestDetail
    {
        public LabTestDetail()
        {
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int LabTestId { get; set; }
        public string LabUnit { get; set; }
        public double? MinRange { get; set; }
        public double? MaxRange { get; set; }
        public string SelectList { get; set; }
        public LabResultType LabResultType { get; set; }
        public bool IsTitle { get; set; }
        public int SortOrder { get; set; }

        [NotMapped]
        public string LabTestName { get; set; }

        [SkipProperty]
        public LabTest LabTest { get; set; }
    }
}
