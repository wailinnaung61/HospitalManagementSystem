using Microsoft.AspNetCore.Mvc;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using X.PagedList;
using EnumExtension = MSIS_HMS.Infrastructure.Enums.EnumExtension;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace MSIS_HMS.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ApplicationDbContext _context;
        private readonly IBranchService _branchService;
        private readonly IUserService _userService;
        private readonly Pagination _pagination;
        private readonly ILogger<DepartmentsController> _logger;
        private IConfiguration Configuration;
        private IHostingEnvironment _hostingEnv;

        public DepartmentsController(IDepartmentRepository departmentRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IBranchService branchService, IUserService userService, IOptions<Pagination> pagination, ILogger<DepartmentsController> logger, IConfiguration _configuration, IHostingEnvironment hostingEnvironment)
        {
            _departmentRepository = departmentRepository;
            _context = context;
            _userManager = userManager;
            _branchService = branchService;
            _userService = userService;
            _pagination = pagination.Value;
            _logger = logger;
            Configuration = _configuration;
            _hostingEnv = hostingEnvironment;
        }

        public ActionResult Index(int? BranchId = null, string DepartmentName = null, int? page = 1, int? TypeId = null)
        {
            Initialize();

            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;

            var branches = _branchService.GetAll();
            var departments = _departmentRepository.GetAll(_userService.Get(User).BranchId).Where(department =>
                (BranchId == null || department.BranchId == BranchId) &&
                (DepartmentName == null || department.Name.ToLower().Contains(DepartmentName.ToLower()) || DepartmentName.ToLower().Contains(department.Name.ToLower())) &&
                (TypeId == null || (int)department.Type == TypeId)).ToList();

            departments.ForEach(x =>
            {
                x.Branch = branches.SingleOrDefault(b => b.Id == x.BranchId);
                x.TypeDescription = EnumExtension.ToDescription(x.Type);
            });

            return View(departments.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public void Initialize()
        {
            var branches = _branchService.GetAll();
            var enumData = from DepartmentTypeEnum e in Enum.GetValues(typeof(DepartmentTypeEnum))
                           select new
                           {
                               ID = (int)e,
                               Name = EnumExtension.ToDescription(e),
                           };
            ViewData["Branches"] = new SelectList(branches, "Id", "Name");
            ViewData["Types"] = new SelectList(enumData, "ID", "Name");
        }


        public ActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Department department)
        {
            try
            {
                ModelState.Remove("BranchId");
                if (ModelState.IsValid)
                {
                    department = await _userManager.AddUserAndTimestamp(department, User, DbEnum.DbActionEnum.Create);
                    var _department = await _departmentRepository.AddAsync(department);
                    if (_department != null)
                    {
                        TempData["notice"] = StatusEnum.NoticeStatus.Success;
                        _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            Initialize();
            return View();
        }

        public ActionResult Edit(int id)
        {
            Initialize();
            var _departments = _departmentRepository.Get(id);
            return View(_departments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Department department)
        {
            try
            {
                ModelState.Remove("BranchId");
                if (ModelState.IsValid)
                {
                    department = await _userManager.AddUserAndTimestamp(department, User, DbEnum.DbActionEnum.Update);
                    var _branch = await _departmentRepository.UpdateAsync(department);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            Initialize();
            return View(department);
        }
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var _department = await _departmentRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetAll(int? DepartmentType)
        {
            var departments = _departmentRepository.GetAll(_userService.Get(User).BranchId).Where(x => DepartmentType == null || (int)x.Type == (int)DepartmentType);
            return Ok(departments);
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
            else { 
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
                Department department = new Department();
                department = await _userManager.AddUserAndTimestamp(department, User, DbEnum.DbActionEnum.Create);
                dt.Columns.Add("BranchId", typeof(Int32));
                dt.Columns.Add("CreatedAt", typeof(DateTime));
                dt.Columns.Add("CreatedBy", typeof(string));
                dt.Columns.Add("UpdatedAt", typeof(DateTime));
                dt.Columns.Add("UpdatedBy", typeof(string));
                foreach (var row in dt.AsEnumerable().ToList())
                {
                    row["BranchId"] = department.BranchId;
                    row["CreatedAt"] = department.CreatedAt;
                    row["CreatedBy"] = department.CreatedBy;
                    row["UpdatedAt"] = department.UpdatedAt;
                    row["UpdatedBy"] = department.UpdatedBy;
                }
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name.
                    sqlBulkCopy.DestinationTableName = "dbo.Department";

                    // Map the Excel columns with that of the database table, this is optional but good if you do
                    // 
                    sqlBulkCopy.ColumnMappings.Add("CreatedAt", "CreatedAt");
                    sqlBulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedAt", "UpdatedAt");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedBy", "UpdatedBy");
                    sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                    sqlBulkCopy.ColumnMappings.Add("Description", "Description");
                    sqlBulkCopy.ColumnMappings.Add("BranchId", "BranchId");
                    sqlBulkCopy.ColumnMappings.Add("Type", "Type");
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
