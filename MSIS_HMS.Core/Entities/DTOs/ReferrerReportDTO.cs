using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class ReferrerReportDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal LabFee { get; set; }
        public decimal OTFee { get; set; }
        public decimal OPDFee { get; set; }        
    }
}
