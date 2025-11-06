using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("VisitType")]
    public class VisitType:BranchEntity
    {
        public VisitType()
        {
        }
        [Required]
        public string Type { get; set; }
        public string Remark { get; set; }

        [SkipProperty]
        public ICollection<Visit> Visits { get; set; }
    }
}
