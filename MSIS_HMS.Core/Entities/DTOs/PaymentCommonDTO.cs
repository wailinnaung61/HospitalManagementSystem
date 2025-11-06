using MSIS_HMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class PaymentCommonDTO
    {
        public string VoucherNo { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string Guardian { get; set; }
        public int Age { get; set; }
        public DateTime DOA { get; set; }
        public DateTime DODC { get; set; }
        public PaymentTypeEnum PaymentType { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public string PaymentTypeName { get; set; }
      
    }
}
