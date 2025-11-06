using MSIS_HMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class VisitPatientDTO
    {
        public string PatientName { get; set; }
        public GenderEnum.Gender PatientGender { get; set; }
        public int PatientAge { get; set; }
        public string DoctorName { get; set; }
        public VisitStatusEnum Status { get; set; }
        public string VisitTypeDesc { get; set; }
        public string VoucherNo { get; set; }
       
    }
}
