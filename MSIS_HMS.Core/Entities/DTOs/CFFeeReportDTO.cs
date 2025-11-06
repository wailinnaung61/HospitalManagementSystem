using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class CFFeeReportDTO
    {
        public string DoctorName { get; set; }
        public int NoOfVisit { get; set; } 
        public int CFFee { get; set; }
    }
}
