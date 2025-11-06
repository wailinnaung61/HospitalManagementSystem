using System;
using System.Collections.Generic;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class PatientDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nrc { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
      
        public DateTime? DateOfBirth { get; set; }
        public string Guardian { get; set; }
        public string Religion { get; set; }
        public string Image { get; set; }
        [SkipProperty]
        public List<Visit> visits { get; set; }
        [SkipProperty]
        public List<MedicalRecord> medicalRecords { get; set; }
        [SkipProperty]
        public List<IPDRecord> iPDRecords { get; set; }
    }
}
