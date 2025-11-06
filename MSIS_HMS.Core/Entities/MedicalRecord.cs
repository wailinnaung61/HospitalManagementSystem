using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("MedicalRecord")]
    public class MedicalRecord : BranchEntity
    {
        public MedicalRecord()
        {
        }

        public MedicalRecord(int? PatientId, int? DoctorId, int? DepartmentType = null,int? IPDRecordId=null)
        {
            if (PatientId != null) this.PatientId = (int)PatientId;
            if (DoctorId != null) this.DoctorId = (int)DoctorId;
            if (DepartmentType != null) this.DepartmentType = (DepartmentTypeEnum)DepartmentType;
            if (IPDRecordId != null) this.IPDRecordId = (int)IPDRecordId;
        }

        public int? VisitId { get; set; }
        public int? IPDRecordId { get; set; }
        public DateTime Date { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string BPSystolic { get; set; }
        public string BPDiastolic { get; set; }
        public string Temperature { get; set; }
        public string Pulse { get; set; }
        public string RespiratoryRate { get; set; }
        public string Weight { get; set; }
        public string Height { get; set; }
        public string GCS { get; set; }
        public string SPO2 { get; set; }
        public DepartmentTypeEnum DepartmentType { get; set; }
        public string DoctorNotes { get; set; }
        public string History { get; set; }
        public string Diagnosis { get; set; }
        public string DoctorNotesImg { get; set; }
        [NotMapped]
        [SkipProperty]
        public string DoctorNotesImgContent { get; set; }
        [NotMapped]
        public string PatientName { get; set; }
        [NotMapped]
        [SkipProperty]
        public int PatientAge { get; set; }
        [NotMapped]
        [SkipProperty]
        public string DateString { get; set; }
        [NotMapped]
        public string PatientAddress { get; set; }
        [NotMapped]
        public DateTime PatientDOB { get; set; }
        [NotMapped]
        public GenderEnum.Gender Gender { get; set; }
        [NotMapped]
        [SkipProperty]
        public string GenderStr { get; set; }
        [NotMapped]
        public string DoctorName { get; set; }
        [NotMapped]
        public string VisitReason { get; set; }

        [SkipProperty]
        public Visit Visit { get; set; }
        [SkipProperty]
        public ICollection<PatientSymptom> PatientSymptoms { get; set; }
        [SkipProperty]
        public ICollection<PatientDiagnostic> PatientDiagnostics { get; set; }
        [SkipProperty]
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}