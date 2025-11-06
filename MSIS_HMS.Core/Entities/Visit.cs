using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Visit")]
    public class Visit : BranchEntity
    {
        public Visit()
        {
        }
        [Required]
        public string VisitNo { get; set; }
        public DateTime Date { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int VisitTypeId { get; set; }
        public int Completed { get; set; }
        public VisitStatusEnum Status { get; set; }
        [Required]
        public string ReasonForVisit { get; set; }
        public string ReasonForCancellation { get; set; }
        public decimal CFFee { get; set; }

        [NotMapped]
        public string PatientName { get; set; }
        [NotMapped]
        public string DoctorName { get; set; }
        [NotMapped]
        public string VisitTypeDesc { get; set; }
        
        [NotMapped]
        [SkipProperty]
        public string StatusDesc { get; set; }

        [SkipProperty]
        public Patient Patient { get; set; }
        [SkipProperty]
        public Doctor Doctor { get; set; }
        [SkipProperty]
        public VisitType VisitType { get; set; }
        [SkipProperty]
        public MedicalRecord MedicalRecord { get; set; }
    }
}
