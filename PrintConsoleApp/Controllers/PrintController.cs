using System.Collections.Generic;
using System.Web.Http;
using System.Drawing.Printing;
using ESC_POS_USB_NET.Printer;
using ESC_POS_USB_NET.Enums;
using System;
using Newtonsoft.Json;
using PrintConsoleApp.Models;

namespace PrintConsoleApp.Controllers
{
    public class PrintController : ApiController
    {
        // GET api/values 
        public string Get(string data)
        {
           
            var slip = JsonConvert.DeserializeObject<Slip>(data); 
            PrinterSettings ps = new PrinterSettings();
            Printer printer = new Printer(ps.PrinterName);
            printer.AlignCenter();
            printer.BoldMode(slip.Hospital);
            printer.NewLine();
            printer.Append(Format(slip.Address, 50));
            printer.Append(slip.Phone);
            printer.NewLine();
            printer.Separator();
            printer.AlignLeft();
            printer.Append("Voucher No: " + slip.VoucherNo);
            printer.Append("Date: " + slip.Date);
            if (!string.IsNullOrEmpty(slip.Patient))
            {
                printer.Append("Patient: " + slip.Patient);
            }
            if (!string.IsNullOrEmpty(slip.Doctor))
            {
                printer.Append("Doctor: " + slip.Doctor);
            }
            printer.Separator();
            printer.Append(String.Format("{0,-25}  {1,8}  {2,11}", "Item Name", "QTY", "Amount"));
            printer.Separator();
            foreach (var item in slip.SlipItems)
            {
                printer.Append(String.Format("{0,-25}  {1,8}  {2,11}", FormatItem(item.ItemName, 25), item.Qty, item.Amount));
            }
            printer.Separator();
            printer.Append(String.Format("{0,35}  {1,11}", "Sub Total", slip.SubTotal));
            printer.Append(String.Format("{0,35}  {1,11}", "Tax", slip.Tax));
            printer.Append(String.Format("{0,35}  {1,11}", "Discount", slip.Discount));
            printer.Append(String.Format("{0,35}  {1,11}", "GrandTotal", slip.GrandTotal));
            printer.Separator();
            printer.FullPaperCut();
            printer.PrintDocument();
            return "";
        }
        public string FormatItem(string text, int index)
        {
            text = "-" + text;
            var result = "";
            var textLength = text.Length;
            var texts = text.Split(' ');
            var lengthOfCurrent = 0;
            var lineNo = 1;
            for (var i = 0; i < texts.Length; i++)
            {
                var txt = (i > 0 ? " " : "") + texts[i];
                var txtLength = txt.Length;
                lengthOfCurrent += txtLength;
                if (lengthOfCurrent > index)
                {
                    result += Environment.NewLine + txt.Trim();
                    lineNo++;
                    lengthOfCurrent = txt.Length - (i > 0 ? 1 : 0);
                }
                else
                {
                    result += txt;
                }

                if (i == texts.Length - 1)
                {
                    result += "".PadRight(lengthOfCurrent < index ? (index) - lengthOfCurrent : 0);
                }
            }

            //Console.WriteLine(result);
            return result;
        }
        public string Format(string text, int index)
        {
            var result = "";
            var textLength = text.Length;
            var texts = text.Split(' ');
            var lengthOfCurrent = 0;
            var lineNo = 1;
            for (var i = 0; i < texts.Length; i++)
            {
                var txt = (i > 0 ? " " : "") + texts[i];
                var txtLength = txt.Length;
                lengthOfCurrent += txtLength;
                if (lengthOfCurrent > index)
                {
                    result += Environment.NewLine + txt.Trim();
                    lineNo++;
                    lengthOfCurrent = txt.Length - (i > 0 ? 1 : 0);
                }
                else
                {
                    result += txt;
                }
            }
            return result;
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values 
        [HttpPost]
        public string Post([FromBody] Slip slip)
        {
            PrinterSettings ps = new PrinterSettings();
            Printer printer = new Printer(ps.PrinterName);
            printer.AlignCenter();
            printer.BoldMode(slip.Hospital);
            printer.NewLine();
            printer.Append(Format(slip.Address, 50));
            printer.Append(slip.Phone);
            printer.NewLine();
            printer.Separator();
            printer.AlignLeft();
            printer.Append("Voucher No: " + slip.VoucherNo);
            printer.Append("Date: " + slip.Date);
            if (!string.IsNullOrEmpty(slip.Patient))
            {
                printer.Append("Patient: " + slip.Patient);
            }
            if (!string.IsNullOrEmpty(slip.Doctor))
            {
                printer.Append("Doctor: " + slip.Doctor);
            }
            printer.Separator();
            printer.Append(String.Format("{0,-25}  {1,8}  {2,11}", "Item Name", "QTY", "Amount"));
            printer.Separator();
            foreach (var item in slip.SlipItems)
            {
                printer.Append(String.Format("{0,-25}  {1,8}  {2,11}", FormatItem(item.ItemName, 25), item.Qty, item.Amount));
            }
            printer.Separator();
            printer.Append(String.Format("{0,35}  {1,11}", "Sub Total", slip.SubTotal));
            printer.Append(String.Format("{0,35}  {1,11}", "Tax", slip.Tax));
            printer.Append(String.Format("{0,35}  {1,11}", "Discount", slip.Discount));
            printer.Append(String.Format("{0,35}  {1,11}", "GrandTotal", slip.GrandTotal));
            printer.Separator();
            printer.FullPaperCut();
            printer.PrintDocument();
            return "";
        }

        // PUT api/values/5 
        public void Put(int id, string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}
