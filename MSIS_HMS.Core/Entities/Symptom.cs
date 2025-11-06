using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Symptom")]
    public class Symptom : Entity
    {
        public Symptom()
        {

        }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? SpecialityId { get; set; }
        public int DoctorId { get; set; }

        [NotMapped]
        public string SpecialityName { get; set; }
        [SkipProperty]
        public Doctor Doctor { get; set; }
        [SkipProperty]
        public ICollection<PatientSymptom> PatientSymptoms { get; set; }
    }
}
