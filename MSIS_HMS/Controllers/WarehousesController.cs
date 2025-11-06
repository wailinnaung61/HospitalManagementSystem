using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using Microsoft.Reporting.NETCore;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using MSIS_HMS.Services;
using X.PagedList;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace MSIS_HMS.Controllers
{
    public class WarehousesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IBranchService _branchService;
        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;
        private readonly IItemService _itemService;
        private readonly IWarehouseService _warehouseService;
        private readonly IBatchService _batchService;
        private readonly IReportService _reportService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration Configuration;
        private IHostingEnvironment _hostingEnv;
        private readonly IUserService _userService;


        public WarehousesController(UserManager<ApplicationUser> userManager, IWarehouseRepository warehouseRepository, IBranchService branchService, IItemService itemService, IWarehouseService warehouseService, IBatchService batchService, ApplicationDbContext context, IOptions<Pagination> pagination,IReportService reportService,IWebHostEnvironment webHostEnvironment,IUserService userService)
        {
            _userManager = userManager;
            _warehouseRepository = warehouseRepository;
            _branchService = branchService;
            _context = context;
            _pagination = pagination.Value;
            _itemService = itemService;
            _warehouseService = warehouseService;
            _batchService = batchService;
            _reportService = reportService;
            _webHostEnvironment = webHostEnvironment;
            _userService = userService;
        }

        public void Initialize(Warehouse warehouse = null)
        {
            ViewData["Branches"] = _branchService.GetSelectListItems(warehouse?.BranchId);
        }

        // GET
        public IActionResult Index(string WareHouseName = null, string Code = null, int? page = 1)
        {
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;

            var warehouses = _warehouseRepository.GetAll(_branchService.GetBranchIdByUser()).Where(warehouse =>
                ((WareHouseName != null && warehouse.Name.ToLower().Contains(WareHouseName.ToLower())) || (WareHouseName == null && warehouse.Name != null)) &&
                ((Code != null && warehouse.Code.ToLower() == Code.ToLower()) || (Code == null && warehouse.Code != null))).ToList();
            var branches = _branchService.GetAll();
            warehouses.ForEach(x => x.Branch = branches.SingleOrDefault(b => b.Id == x.BranchId));
            return View(warehouses.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Warehouse warehouse)
        {
            try
            {
                ModelState.Remove("BranchId");
                if (ModelState.IsValid)
                {
                    warehouse = await _userManager.AddUserAndTimestamp(warehouse, User, DbEnum.DbActionEnum.Create);
                    var _warehouse = await _warehouseRepository.AddAsync(warehouse);
                    if (_warehouse != null)
                    {
                        TempData["notice"] = StatusEnum.NoticeStatus.Success;
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Initialize();
            return View();
        }

        public IActionResult Edit(int id)
        {
            Initialize();
            var _warehouse = _warehouseRepository.Get(id);
            return View(_warehouse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Warehouse warehouse)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    warehouse = await _userManager.AddUserAndTimestamp(warehouse, User, DbEnum.DbActionEnum.Update);
                    var _warehouse = await _warehouseRepository.UpdateAsync(warehouse);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Initialize();
            return View(warehouse);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var _warehouse = await _warehouseRepository.DeleteAsync(id);
            TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Stock(int? page = 1, int? WarehouseId = null, int? ItemId = null, int? BatchId = null, DateTime? StartExpiryDate = null, DateTime? EndExpiryDate = null)
        {
            var warehouseItems = _warehouseRepository.GetWarehouseItemDTOs(_branchService.GetBranchIdByUser(), WarehouseId, ItemId, BatchId, null, StartExpiryDate, EndExpiryDate);
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Warehouses"] = _warehouseService.GetSelectListItems(WarehouseId);
            ViewData["Items"] = _itemService.GetSelectListItems(ItemId);
            ViewData["Batches"] = _batchService.GetSelectListItems(BatchId);
            return View(warehouseItems.ToPagedList((int)page, pageSize));
        }
        public IActionResult WarehouseStockReport(int? page = 1, int? WarehouseId = null, int? ItemId = null, int? BatchId = null)
        {
            var warehouseItems = _warehouseRepository.GetWarehouseItemReport(_branchService.GetBranchIdByUser(), WarehouseId, ItemId, BatchId, null, null, null);
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Warehouses"] = _warehouseService.GetSelectListItems(WarehouseId);
            ViewData["Items"] = _itemService.GetSelectListItems(ItemId);
            ViewData["Batches"] = _batchService.GetSelectListItems(BatchId);
            TempData["WarehouseId"] =WarehouseId ;
            TempData["BatchId"] = BatchId;
            TempData["ItemId"] = ItemId;
            return View(warehouseItems.ToPagedList((int)page, pageSize));
        }

        public IActionResult GetWarehouseItems(int WarehouseId)
        {
            var warehouseItems = _warehouseRepository.GetItemsFromWarehouse((int)_branchService.GetBranchIdByUser(), WarehouseId);
            return Ok(warehouseItems);
        }
        public IActionResult GetQtyOfWarehouseItem(int WarehouseId,int ItemId,int BatchId)
        {
            var warehouseItemQty = _context.WarehouseItems.Where(x => x.WarehouseId == WarehouseId && x.ItemId == ItemId && x.BatchId == BatchId).FirstOrDefault();
            return Ok(warehouseItemQty.Qty);
        }
        public IActionResult GetBatchesOfWarehouseItem(int WarehouseId, int ItemId)
        {
            var batches = _warehouseRepository.GetBatchesOfWarehouseItem((int)_branchService.GetBranchIdByUser(), WarehouseId, ItemId);
            return Ok(batches);
        }
        public IActionResult GetWarehouseItemByBatchId(int? warehouseId, int? batchId, int? itemId)
        {
            var warehouseItem = _context.WarehouseItems.Where(x => x.WarehouseId == warehouseId && x.BatchId == batchId && x.ItemId == itemId).FirstOrDefault();
            return Ok(warehouseItem);
        }

        public IActionResult Report(int? page = 1, int? WarehouseId = null, int? ItemId = null, int? LocationId = null)
        {
            var warehouseItems = _warehouseRepository.GetWarehouseItemLocationsDTO(_branchService.GetBranchIdByUser(), WarehouseId, ItemId, LocationId);
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Warehouses"] = _warehouseService.GetSelectListItems(WarehouseId);
            ViewData["Items"] = _itemService.GetSelectListItems(ItemId);
            var locations = _context.Locations.Where(x => x.IsDelete == false).ToList();
            ViewData["Locations"] = new SelectList(locations, "Id", "Name");
            return View(warehouseItems.ToPagedList((int)page, pageSize));
        }

        [HttpGet]
        public IActionResult ExcelExport(int? page = 1, int? WarehouseId = null, int? ItemId = null, int? LocationId = null)
        {
            var warehouseItems = _warehouseRepository.GetWarehouseItemLocationsDTO(_branchService.GetBranchIdByUser(), WarehouseId, ItemId, LocationId);
           // var result = _reportService.WarehouseItemByExRemindDayexcelExport(warehouseItems);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("WarehouseReport");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Warhouse";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 2).Value = "Item(Code)";
                worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 3).Value = "Batch";
                worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 4).Value = "Qty(< ExpirationRemind Days)";
                worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 5).Value = "ExpirationRemind Days";
                worksheet.Cell(currentRow, 5).Style.Font.Bold = true;

                foreach (var w in warehouseItems)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = w.WarehouseName;
                    worksheet.Cell(currentRow, 2).Value = w.ItemName+"("+w.ItemCode+")";
                    worksheet.Cell(currentRow, 3).Value = w.BatchName;
                    worksheet.Cell(currentRow, 4).Value = w.Qty;
                    worksheet.Cell(currentRow, 5).Value = w.ExpirationRemindDay;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                   
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Warehouse.xlsx");
                }
                
            }
          
        }
        [HttpGet]
        public IActionResult StockExcelExport(int? WarehouseId = null, int? ItemId = null, int? BatchId = null, DateTime? StartExpiryDate = null, DateTime? EndExpiryDate = null)
        {
            var warehouseItems = _warehouseRepository.GetWarehouseItemDTOs(_branchService.GetBranchIdByUser(), WarehouseId, ItemId, BatchId, null, StartExpiryDate, EndExpiryDate);
            // var result = _reportService.WarehouseItemByExRemindDayexcelExport(warehouseItems);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("WarehouseStockReport");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Warhouse";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 2).Value = "Item(Code)";
                worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 3).Value = "Batch";
                worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 4).Value = "Qty";
                worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 5).Value = "Expiry Date";
                worksheet.Cell(currentRow, 5).Style.Font.Bold = true;

                foreach (var w in warehouseItems)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = w.WarehouseName;
                    worksheet.Cell(currentRow, 2).Value = w.ItemName + "(" + w.ItemCode + ")";
                    worksheet.Cell(currentRow, 3).Value = w.BatchName;
                    worksheet.Cell(currentRow, 4).Value = w.Qty;
                    worksheet.Cell(currentRow, 5).Value = w.ExpiryDate;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WarehouseStock.xlsx");
                }

            }

        }

        public IActionResult DownloadReport()
        {
            var page = TempData["Page"];
            int? warehouseId = null;
            int? batchId = null;
            int? itemId = null;
            if(TempData["WarehouseId"]!=null)
            {
               warehouseId = (int)TempData["WarehouseId"];
            }
            if(TempData["BatchId"]!=null)
            {
                batchId = (int)TempData["BatchId"];
            }
            if(TempData["ItemId"]!=null)
            {
                itemId= (int)TempData["ItemId"];
            }
                      
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                var user = _userService.Get(User);
                List<WarehouseItemDTO> warehouseItems = new List<WarehouseItemDTO>();
                List<Branch> branches = new List<Branch>();
                var branch = _branchService.GetBranchById((int)user.BranchId);
                branches.Add(branch);
                // var user = _userService.Get(User);
                warehouseItems = _warehouseRepository.GetWarehouseItemReport(_branchService.GetBranchIdByUser(), warehouseId, null, batchId, null, null, null);
              
                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.DataSources.Add(new ReportDataSource("dsWarehouseStock", warehouseItems));

                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\WarehouseStockReport.rdlc";
                var pdf = report.Render(renderFormat); 
                TempData["Page"] = page;
                TempData["WarehouseId"] = warehouseId;
                TempData["BatchId"] = batchId;
                TempData["ItemId"] = itemId;
               
                return File(pdf, mimetype, "WarehouseStockReport_" + DateTime.Now + "." + extension);
            }
        }

        public ActionResult Upload()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "UploadExcel";
            string webRootPath = _hostingEnv.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            StringBuilder sb = new StringBuilder();
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                    }
                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    int cellCount = headerRow.LastCellNum;
                    sb.Append("<table class='table table-bordered'><tr>");
                    for (int j = 0; j < cellCount; j++)
                    {
                        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                        sb.Append("<th>" + cell.ToString() + "</th>");
                    }
                    sb.Append("</tr>");
                    sb.AppendLine("<tr>");
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        }
                        sb.AppendLine("</tr>");
                    }
                    sb.Append("</table>");
                }
            }
            return this.Content(sb.ToString());
        }

        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcelFile(IFormFile FormFile)
        {
            if(FormFile == null)
            {
                return RedirectToAction("Import");
            }
            else
            {

            
            //get file name
            var filename = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition).FileName.Trim('"');
            //get path
            var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            //create directory "Uploads" if it doesn't exists
            if (!Directory.Exists(MainPath))
            {
                Directory.CreateDirectory(MainPath);
            }

            //get file path 
            var filePath = Path.Combine(MainPath, FormFile.FileName);
            using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
            {
                await FormFile.CopyToAsync(stream);
            }

            //get extension
            string extension = Path.GetExtension(filename);


            string conString = string.Empty;

            switch (extension)
            {
                case ".xls": //Excel 97-03.
                    conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                    break;
                case ".xlsx": //Excel 07 and above.
                    conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                    break;
            }

            DataTable dt = new DataTable();
            conString = string.Format(conString, filePath);

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        connExcel.Close();

                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dt);
                        connExcel.Close();
                    }
                }
            }
            //your database connection string
            conString = this.Configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(conString))
            {
                Warehouse warehouse = new Warehouse();
                warehouse = await _userManager.AddUserAndTimestamp(warehouse, User, DbEnum.DbActionEnum.Create);
                dt.Columns.Add("BranchId", typeof(Int32));
                dt.Columns.Add("IsDelete", typeof(bool));
                dt.Columns.Add("CreatedAt", typeof(DateTime));
                dt.Columns.Add("CreatedBy", typeof(string));
                dt.Columns.Add("UpdatedAt", typeof(DateTime));
                dt.Columns.Add("UpdatedBy", typeof(string));
                foreach (var row in dt.AsEnumerable().ToList())
                {
                    row["BranchId"] = warehouse.BranchId;
                    row["IsDelete"] = warehouse.IsDelete;
                    row["CreatedAt"] = warehouse.CreatedAt;
                    row["CreatedBy"] = warehouse.CreatedBy;
                    row["UpdatedAt"] = warehouse.UpdatedAt;
                    row["UpdatedBy"] = warehouse.UpdatedBy;
                }
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name.
                    sqlBulkCopy.DestinationTableName = "dbo.Warehouse";

                    // Map the Excel columns with that of the database table, this is optional but good if you do
                    // 
                    
                    sqlBulkCopy.ColumnMappings.Add("IsDelete", "IsDelete");
                    sqlBulkCopy.ColumnMappings.Add("BranchId", "BranchId");
                    sqlBulkCopy.ColumnMappings.Add("CreatedAt", "CreatedAt");
                    sqlBulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedAt", "UpdatedAt");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedBy", "UpdatedBy");
                    sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                    sqlBulkCopy.ColumnMappings.Add("Code", "Code");
                    sqlBulkCopy.ColumnMappings.Add("Location", "Location");


                    con.Open();
                    sqlBulkCopy.WriteToServer(dt);
                    con.Close();
                }
            }
                //if the code reach here means everthing goes fine and excel data is imported into database
            }


            return RedirectToAction("Index");

        }

    }
}