using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("OperationService")]
    public class OperationService
    {
        public OperationService()
        {

        }
        public int Id { get; set; }
        public int OperationTreaterId { get; set; }
        public int ServiceId { get; set; }
        public decimal Fee { get; set; }
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
        public bool IsFOC { get; set; } // FOC
        public int SortOrder { get; set; }

    
        [NotMapped]
        public string ServiceName { get; set; }

        [SkipProperty]
        public OperationTreater OperationTreater { get; set; }
        [SkipProperty]
        public Service Service { get; set; }
      
    }
}
