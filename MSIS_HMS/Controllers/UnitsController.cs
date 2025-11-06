using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Repositories;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using X.PagedList;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Data;
using System.Data.OleDb;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace MSIS_HMS.Controllers
{
    public class UnitsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBranchRepository _branchRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IBranchService _branchService;
        private readonly Pagination _pagination;
        private readonly IConfiguration Configuration;
        private IHostingEnvironment _hostingEnv;
        public UnitsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IBranchRepository branchRepository, IUnitRepository unitRepository, IBranchService branchService,IOptions<Pagination> pagination, IConfiguration _configuration, IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _branchRepository = branchRepository;
            _unitRepository = unitRepository;
            _branchService = branchService;
            _pagination = pagination.Value;
            Configuration = _configuration;
            _hostingEnv = hostingEnvironment;
        }

        public void Initialize(Unit unit = null)
        {
            ViewData["Branches"] = _branchService.GetSelectListItems(unit?.BranchId);
        }

        // GET
        public IActionResult Index(int? page=1,string UnitName=null,string ShortName=null)
        {
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var units = _unitRepository.GetAll(_branchService.GetBranchIdByUser()).Where(unit=>
                ((UnitName != null && unit.Name.ToLower().Contains(UnitName.ToLower())) || (UnitName == null && unit.Name != null)) &&
                ((ShortName != null && unit.ShortForm.ToLower() == ShortName.ToLower()) || (ShortName == null && unit.ShortForm != null))).ToList();
            
            var branches = _branchService.GetAll();
            units.ForEach(x => x.Branch = branches.SingleOrDefault(b => b.Id == x.BranchId));
            return View(units.OrderBy(x => x.Branch.CreatedAt).ThenBy(x => x.UnitLevel).ThenBy(x => x.Name).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Unit unit)
        {
            try
            {
                ModelState.Remove("BranchId");
                if (ModelState.IsValid)
                {
                    unit = await _userManager.AddUserAndTimestamp(unit, User, DbEnum.DbActionEnum.Create);
                    var _unit = await _unitRepository.AddAsync(unit);
                    if (_unit != null)
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
            Initialize(unit);
            return View();
        }

        public IActionResult Edit(int id)
        {
            var unit = _unitRepository.Get(id);
            Initialize(unit);
            return View(unit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Unit unit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unit = await _userManager.AddUserAndTimestamp(unit, User, DbEnum.DbActionEnum.Update);
                    var _unit = await _unitRepository.UpdateAsync(unit);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Initialize(unit);
            return View(unit);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var _unit = await _unitRepository.DeleteAsync(id);
            TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetAll(int BranchId)
        {
            var units = _unitRepository.GetAll(_branchService.GetBranchIdByUser());
            return Ok(units.OrderBy(x => x.UnitLevel).ThenBy(x => x.Name).ToList());
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
                Unit unit = new Unit();
                unit = await _userManager.AddUserAndTimestamp(unit, User, DbEnum.DbActionEnum.Create);
                dt.Columns.Add("BranchId", typeof(Int32));
                dt.Columns.Add("IsDelete", typeof(bool));
                dt.Columns.Add("CreatedAt", typeof(DateTime));
                dt.Columns.Add("CreatedBy", typeof(string));
                dt.Columns.Add("UpdatedAt", typeof(DateTime));
                dt.Columns.Add("UpdatedBy", typeof(string));
                foreach (var row in dt.AsEnumerable().ToList())
                {
                    row["BranchId"] = unit.BranchId;
                    row["IsDelete"] = unit.IsDelete;
                    row["CreatedAt"] = unit.CreatedAt;
                    row["CreatedBy"] = unit.CreatedBy;
                    row["UpdatedAt"] = unit.UpdatedAt;
                    row["UpdatedBy"] = unit.UpdatedBy;
                }
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name.
                    sqlBulkCopy.DestinationTableName = "dbo.Unit";

                    // Map the Excel columns with that of the database table, this is optional but good if you do
                    // 
                    
                    sqlBulkCopy.ColumnMappings.Add("IsDelete", "IsDelete");
                    sqlBulkCopy.ColumnMappings.Add("BranchId", "BranchId");
                    sqlBulkCopy.ColumnMappings.Add("CreatedAt", "CreatedAt");
                    sqlBulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedAt", "UpdatedAt");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedBy", "UpdatedBy");
                    sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                    sqlBulkCopy.ColumnMappings.Add("ShortForm", "ShortForm");
                    sqlBulkCopy.ColumnMappings.Add("Description", "Description");
                    sqlBulkCopy.ColumnMappings.Add("Remark", "Remark");
                    sqlBulkCopy.ColumnMappings.Add("UnitLevel", "UnitLevel");


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