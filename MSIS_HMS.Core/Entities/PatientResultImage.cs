using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    public class PatientResultImage:BranchEntity
    {
       public PatientResultImage()
        {

        }
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string AttachmentPath { get; set; }
        public string Remark { get; set; }
        [NotMapped]
        public string PatientName { get; set; }
        [SkipProperty]
        public Patient Patient { get; set; }
    }
}
