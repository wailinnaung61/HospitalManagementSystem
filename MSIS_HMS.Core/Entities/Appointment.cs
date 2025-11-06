using System;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Appointment")]
    public class Appointment : BranchEntity
    {
        public Appointment()
        {
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int AppointmentTypeId { get; set; }
        public AppointmentStatusEnum Status { get; set; }
        public string Notes { get; set; }
        [NotMapped]
        public string DoctorName { get; set; }
        [NotMapped]
        public string PatientName { get; set; }
        [NotMapped]
        public string AppointmentTypeName { get; set; }
        [SkipProperty]
        public Patient Patient { get; set; }
        [SkipProperty]
        public Doctor Doctor { get; set; }
        [SkipProperty]
        public AppointmentType AppointmentType { get; set; }
    }
}
