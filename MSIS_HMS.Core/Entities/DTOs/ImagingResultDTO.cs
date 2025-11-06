using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class ImagingResultDTO
    {
        public int LabResultFileId { get; set; }
        public int LabResultId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string ResultNo { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string AttachmentPath { get; set; }
    }
}
