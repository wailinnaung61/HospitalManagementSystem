using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MSIS_HMS.Core.Entities.Attributes;
using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;
using static MSIS_HMS.Core.Enums.GenderEnum;

namespace MSIS_HMS.Core.Entities
{
    [Table("LabPerson")]
    public class LabPerson : BranchEntity
    {
        public LabPerson()
        {

        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
        [Required]
        public LabPersonTypeEnum Type { get; set; }
        public string Remark { get; set; }
        public int? DoctorId { get; set; } // If lab person is a registered doctor from the hospital, select the doctor Id
        [Required]
        public int DepartmentId { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }
        [NotMapped]
        public string DoctorName { get; set; }

        [SkipProperty]
        public Department Department { get; set; }
        [SkipProperty]
        public Doctor Doctor { get; set; }
        [SkipProperty]
        public ICollection<LabPerson_LabTest> LabPerson_LabTests { get; set; }
        [SkipProperty]
        public ICollection<LabResult> LabResultsByTechnicians { get; set; }
        [SkipProperty]
        public ICollection<LabResult> LabResultsByConsultants { get; set; }
    }
}
