using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("LabResultFile")]
    public class LabResultFile
    {
        public LabResultFile()
        {
            
        }
        public int Id { get; set; }
        public int LabResultId { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string AttachmentPath { get; set; }
        public string Remark { get; set; }
        [NotMapped]
        public string LabResultNo { get; set; }
        [SkipProperty]
        public LabResult LabResult { get; set; }

    }
}
