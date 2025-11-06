using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("OperationType")]
    public class OperationType : BranchEntity
    {
        public OperationType()
        {
        }

        [Required]
        public string Name { get; set; }
        public string Code { get; set; }
        public string Remark { get; set; }
        public int EstimateTime { get; set; }

       
    }
}
