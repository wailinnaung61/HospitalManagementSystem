using System;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("PatientSymptom")]
    public class PatientSymptom
    {
        public PatientSymptom()
        {
        }

        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public int? SymptomId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string FromDate { get; set; }
        public int SortOrder { get; set; }

        //[NotMapped]
        //public string SymptomDesc { get; set; }

        [SkipProperty]
        public MedicalRecord MedicalRecord { get; set; }
        [SkipProperty]
        public Symptom Symptom { get; set; }
    }
}
