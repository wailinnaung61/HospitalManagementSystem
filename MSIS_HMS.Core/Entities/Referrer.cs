using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Referrer")]
    public class Referrer:BranchEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        
        public string Township { get; set; }
        
        public string City { get; set; }

        public FeeTypeEnum FeeType { get; set; }

        public decimal Fee { get; set; }

        [SkipProperty]
        public ICollection<OrderService> OrderServices { get; set; }
        [SkipProperty]
        public ICollection<Patient> Patients { get; set; }
    }
}
