using System;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("PatientDiagnostic")]
    public class PatientDiagnostic
    {
        public PatientDiagnostic()
        {
        }

        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public int? DiagnosticId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public int SortOrder { get; set; }

        //[NotMapped]
        //public string DiagnosticDesc { get; set; }

        [SkipProperty]
        public MedicalRecord MedicalRecord { get; set; }
        [SkipProperty]
        public Diagnostic Diagnostic { get; set; }
    }
}
