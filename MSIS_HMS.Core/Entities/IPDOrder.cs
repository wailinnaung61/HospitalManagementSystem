using System;
using MSIS_HMS.Core.Entities.Base;
using System.Collections.Generic;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MSIS_HMS.Core.Entities
{
    [Table("IPDOrder")]
    public class IPDOrder : Entity
    {
        public DateTime Date { get; set; }
        [Required]
        public string VoucherNo { get; set; }
        public int IPDRecordId { get; set; }
        public decimal CFFee { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal Balance { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public string Remark { get; set; }

        [SkipProperty]
        public IPDRecord IPDRecord { get; set; }
        [SkipProperty]
        public ICollection<IPDOrderService> IPDOrderServices { get; set; }
        [SkipProperty]
        public ICollection<IPDOrderItem> IPDOrderItems { get; set; }
        [SkipProperty]
        public ICollection<IPDRound> IPDRounds { get; set; }
        [SkipProperty]
        public ICollection<IPDStaff> IPDStaffs { get; set; }
        [SkipProperty]
        public ICollection<IPDFood>  IPDFoods{ get; set; }








    }
}
