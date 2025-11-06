using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Order")]
    public class Order : BranchEntity
    {
        public Order()
        {
        }

        public DateTime Date { get; set; }
        [Required]
        public string VoucherNo { get; set; }
        public int OutletId { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public decimal CFFee { get; set; }
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
        public string DoctorName { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        public string Address { get; set; }
        [NotMapped]
        public string Phone { get; set; }
        [NotMapped]
        public string OutletName { get; set; }
        [NotMapped]
        public decimal? ReferrerFee { get; set; }
        [NotMapped]
        public int? ConsultantFee { get; set; }
        [NotMapped]
        public decimal? ClinicFee { get; set; }
        [SkipProperty]
        public Outlet Outlet { get; set; }
        [SkipProperty]
        public Patient Patient { get; set; }
        [SkipProperty]
        public Doctor Doctor { get; set; }
        [SkipProperty]
        public ICollection<OrderItem> OrderItems { get; set; }
        [SkipProperty]
        public ICollection<OrderService> OrderServices { get; set; }
    }
}
