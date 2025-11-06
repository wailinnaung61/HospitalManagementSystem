using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
   public class LabResultDetailDTO
    {
        public int testId { get; set; }
        public string Name { get; set; }
        public string Result { get; set; }
        public string LabUnit { get; set; }
        public string ReferenceRange { get; set; }
        public string Remark { get; set; }
    }
}
