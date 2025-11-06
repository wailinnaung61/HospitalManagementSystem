using MSIS_HMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class DoctorHistoryDTO
    {
        public string DoctorName { get; set; }
        public int BookPatient { get; set; }
        public int CompletedPatient { get; set; }
      
    }
}
