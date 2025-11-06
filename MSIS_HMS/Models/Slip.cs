using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSIS_HMS.Models
{
    public class Slip
    {
        public int Id { get; set; }
        public string Hospital { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string VoucherNo { get; set; }
        public string Date { get; set; }
        public string Doctor { get; set; }
        public string Patient { get; set; }
        public string SubTotal { get; set; }
        public string Tax { get; set; }
        public string Discount { get; set; }
        public string GrandTotal { get; set; }
        public List<SlipItem> SlipItems { get; set; }
    }

    public class SlipItem
    {
        public string ItemName { get; set; }
        public string Qty { get; set; }
        public string Amount { get; set; }

        public SlipItem(string ItemName, string Qty, string Amount)
        {
            this.ItemName = ItemName;
            this.Qty = Qty;
            this.Amount = Amount;
        }
    }
}
