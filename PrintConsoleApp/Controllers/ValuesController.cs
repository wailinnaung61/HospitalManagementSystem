using System.Collections.Generic;
using System.Web.Http;
using System.Drawing.Printing;
using ESC_POS_USB_NET.Printer;
using ESC_POS_USB_NET.Enums;
using System;

namespace PrintConsoleApp.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values 
        public IEnumerable<string> Get()
        {
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("");
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Add("Authorization", authorizationKey);
            //    HttpResponseMessage response = await client.GetAsync(Url);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        string content = await response.Content.ReadAsStringAsync();
            //        //UrlMetricsResponse mozResonse = JsonConvert.DeserializeObject<UrlMetricsResponse>(content);
            //        dynamic dynObj = JsonConvert.DeserializeObject(content);
            //        CompanyDetails = JsonConvert.SerializeObject(dynObj);
            //    }

            //}
            var m_PrintDocument = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            //m_PrintDocument.PrinterSettings.PrinterName = ps.PrinterName;
            //m_PrintDocument.Print();

            Printer printer = new Printer(ps.PrinterName);
            printer.AlignCenter();
            printer.BoldMode("Royal Hospital");
            printer.NewLine();
            printer.Append("No.14, Baho Road, Sanchaung Tsp, Yangon,\nMyanmar\n");
            printer.Append("(+951) 2304999");
            printer.NewLine();
            printer.Separator();
            printer.AlignLeft();
            printer.Append("Voucher No: V-00150");
            printer.Append("Date: 22-04-2022");
            printer.Append("Doctor: Dr. Smith Watson");
            printer.Separator();
            printer.Append(String.Format("{0,-25}  {1,8}  {2,11}", "Item Name", "QTY", "Amount"));
            printer.Separator();
            printer.Append(String.Format("{0,-25}  {1,8}  {2,11}", "Biogesic Tab 40mg", "3 Crd", "1,500"));
            printer.Append(String.Format("{0,-25}  {1,8}  {2,11}", "Paracetamol 500mg", "1 Box", "5,200"));
            printer.Append(String.Format("{0,-25}  {1,8}  {2,11}", "Myobiolosym Synthetic\nAtrovastatin Capsulated\nTablet 10 mg", "5 Tab", "3,300"));
            printer.Separator();
            printer.Append(String.Format("{0,35}  {1,11}", "Sub Total", "10,000"));
            printer.Append(String.Format("{0,35}  {1,11}", "Tax", "500"));
            printer.Append(String.Format("{0,35}  {1,11}", "Discount", "0"));
            printer.Append(String.Format("{0,35}  {1,11}", "GrandTotal", "10,500"));
            printer.Separator();
            printer.FullPaperCut();
            printer.PrintDocument();

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values 
        [HttpPost]
        public string Post(string value)
        {
            return value;
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
