using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class PatientImageDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public List<string> Name { get; set; }
        public List<string> AttachmentPath { get; set; }
        public string Remark { get; set; }
    }
}
