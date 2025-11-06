using System;
namespace MSIS_HMS.Models
{
    public class VoucherSetting
    {
        public VoucherSetting()
        {

        }
        public VoucherSetting(bool UseVoucherFormat, string Format, int Number)
        {
            this.UseVoucherFormat = UseVoucherFormat;
            this.Format = Format;
            this.Number = Number;
        }
        public bool UseVoucherFormat { get; set; }
        public string Format { get; set; }
        public int Number { get; set; }
    }
}
