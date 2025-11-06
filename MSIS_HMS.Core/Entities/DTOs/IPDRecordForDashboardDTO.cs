using System;
using System.Collections.Generic;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.DTOs
{
   public class IPDRecordForDashboardDTO
    {
       
        public int Days { get; set; }
        public string PatientName { get; set; }
        public string WardName { get; set; }
        public string RoomNo { get; set; }
        public string BedNo { get; set; }
    }
}
