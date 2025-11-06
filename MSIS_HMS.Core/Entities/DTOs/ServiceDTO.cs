using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Entities.DTOs
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
        public int Qty { get; set; }
        public decimal Amount { get; set; }
        public string OutletName { get; set; }
        public decimal? ReferrerFee { get; set; }

    }
}
