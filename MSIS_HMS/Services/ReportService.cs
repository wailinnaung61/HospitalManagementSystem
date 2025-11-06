using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MSIS_HMS.Core.Entities.DTOs;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using OfficeOpenXml.Style;
using MSIS_HMS.Interfaces;

namespace MSIS_HMS.Services
{
    public class ReportService : IReportService
    {
        public ExcelPackage WarehouseItemByExRemindDayexcelExport(List<WarehouseItemLocationDTO> warehouseItems)
        {

            //var res = ceilings.GroupBy(x => x.SubPTGDesc).Select
            //           (x => new {
            //               SubPTGDesc = x.Key,
            //               TotalProjectUnit = x.Sum(xa => xa.ProjectUnit),
            //               TotalProjectSub = x.Sum(xa => xa.ProjectSub),
            //               TotalProjectTotal = x.Sum(xa => xa.ProjectTotal),
            //               TotalAmount = x.Sum(xa => xa.ProposalTotal),
            //               CeilingUnit = x.Sum(xa => xa.CeilingUnit),
            //               CeilingSub = x.Sum(xa => xa.CeilingSub),
            //               CeilingTotal = x.Sum(xa => xa.CeilingTotal),
            //               CeilingAmount = x.Sum(xa => xa.CeilingAmount),
            //           }).ToList();

            var package = new ExcelPackage();
            package.Workbook.Properties.Title = "Stock On Hand";
            package.Workbook.Properties.Author = "";
            package.Workbook.Properties.Subject = "Stock";


            var worksheet = package.Workbook.Worksheets.Add(warehouseItems.GroupBy(x => x.ItemName).Select(x => x.FirstOrDefault()).Distinct().ToString());

            worksheet.Cells[1, 1].Value = "No.";
            worksheet.Cells[1, 2].Value = "Warehouse Name";
            worksheet.Cells[1, 3].Value = "Item Name";
            worksheet.Cells[1, 4].Value = "On Hand";

            FormatCommon(worksheet, false);

            return package;


        }
        public static void FormatCommon(ExcelWorksheet worksheet, bool wrap = true)
        {
            var allCells = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column];
            allCells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            allCells.Style.WrapText = wrap;
            var cellFont = allCells.Style.Font;
            //cellFont.SetFromFont(new Font("Pyidaungsu", 11));
        }
    }
}
