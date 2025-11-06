using System;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Schedule")]
    public class Schedule: BranchEntity
    {
        public Schedule()
        {
        }

        public int DoctorId { get; set; }
        public int DepartmentId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }

        [NotMapped]
        public string DoctorName { get; set; }
        [NotMapped]
        public string DepartmentName { get; set; }
        [NotMapped]
        [SkipProperty]
        public string DayOfWeekName { get; set; }

        [SkipProperty]
        public Doctor Doctor { get; set; }
        [SkipProperty]
        public Department Department { get; set; }
    }
}
