using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Department")]
    public class Department : BranchEntity
    {
        public Department()
        {
        }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public DepartmentTypeEnum Type { get; set; }
        [SkipProperty]
        [NotMapped]
        public string TypeDescription { get; set; }

        [SkipProperty]
        public ICollection<Schedule> Schedules { get; set; }
        [SkipProperty]
        public ICollection<IPDRecord> IPDRecords { get; set; }
        [SkipProperty]
        public ICollection<LabPerson> LabPersons { get; set; }
    }
}
