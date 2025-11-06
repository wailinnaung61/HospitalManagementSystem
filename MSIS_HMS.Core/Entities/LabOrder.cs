using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("LabOrder")]
    public class LabOrder : BranchEntity
    {
        public LabOrder()
        {
        }

        public DateTime Date { get; set; }
        [Required]
        public string VoucherNo { get; set; }
        public int PatientId { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal Balance { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public string Remark { get; set; }

        [NotMapped]
        public string PatientName { get; set; }
        [NotMapped]
        public string PatientGuardian { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        public string BranchAddress { get; set; }
        [NotMapped]
        public string BranchPhone { get; set; }
        [NotMapped]
        [SkipProperty]
        public string LabTest { get; set; }
        [SkipProperty]
        public Patient Patient { get; set; }
        [SkipProperty]
        public ICollection<LabOrderTest> LabOrderTests { get; set; }
    }
}
