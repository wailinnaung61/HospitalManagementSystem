using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MSIS_HMS.Controllers
{
    public class StaffsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IUserService _userService;
        private readonly IPositionRepository _positionRepository;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration Configuration;
        private IHostingEnvironment _hostingEnv;

        public StaffsController(UserManager<ApplicationUser> userManager, IDepartmentRepository departmentRepository, IStaffRepository staffRepository, IUserService userService,IPositionRepository positionRepository, ApplicationDbContext context, IConfiguration _configuration, IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _departmentRepository = departmentRepository;
            _staffRepository = staffRepository;
            _userService = userService;
            _positionRepository = positionRepository;
            _context = context;
            Configuration = _configuration;
            _hostingEnv = hostingEnvironment;
        }
        public void Initialize()
        {
            var position = _context.Positions.Where(x => x.IsDelete == false).ToList();
            ViewData["Positions"] = new SelectList(position, "Id", "Name");
            var departments = _departmentRepository.GetAll(_userService.Get(User).BranchId);
            ViewData["Departments"] = new SelectList(departments, "Id", "Name");
        }
        public IActionResult Index()
        {
            var staff = _staffRepository.GetAll(_userService.Get(User).BranchId);
            //var departments = _departmentRepository.GetAll(_userService.Get(User).BranchId);
            //var position = _positionRepository.GetAll();
            //staff.ForEach(x => x.Department = departments.SingleOrDefault(a => a.Id == x.DepartmentId));
            //staff.ForEach(x => x.Position = position.SingleOrDefault(b => b.Id == x.PositionId));
            return View(staff);
        }
        public ActionResult Create()
        {
            Initialize();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    staff = await _userManager.AddUserAndTimestamp(staff, User, DbEnum.DbActionEnum.Create);
                    var _staff = await _staffRepository.AddAsync(staff);
                    if (_staff != null)
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
        public ActionResult Edit(int id)
        {
            Initialize();
            var _staff = _staffRepository.Get(id);
            return View(_staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Staff staff)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    staff = await _userManager.AddUserAndTimestamp(staff, User, DbEnum.DbActionEnum.Update);
                    var _staff = await _staffRepository.UpdateAsync(staff);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Initialize();
            return View(staff);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var _staff = await _staffRepository.DeleteAsync(id);
            TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetAll()
        {
            var doctors = _staffRepository.GetAll(_userService.Get(User).BranchId);
            return Ok(doctors);
        }
        public IActionResult GetAllByPosition(int PositionId)
        {
            var staffs = _staffRepository.GetAllByPosition(_userService.Get(User).BranchId,PositionId);
            return Ok(staffs);
        }
        public IActionResult GetStaffTypeEnum()
        {
            List<OTStaffTypeEnumDTO> oTStaffTypeEnumDTOs = new List<OTStaffTypeEnumDTO>();
            var staffTypeEnum = Enum.GetValues(typeof(MSIS_HMS.Core.Enums.OTStaffTypeEnum))
            .Cast<MSIS_HMS.Core.Enums.OTStaffTypeEnum>().ToDictionary(k => k.ToString(), v => (int)v);
            //.Select(v => v.ToString()).ToList();
            foreach (var e in staffTypeEnum)
            {
                OTStaffTypeEnumDTO oTStaffTypeEnumDTO = new OTStaffTypeEnumDTO();
                oTStaffTypeEnumDTO.Name = e.Key;
                oTStaffTypeEnumDTO.value = e.Value;
                oTStaffTypeEnumDTOs.Add(oTStaffTypeEnumDTO);

            }
            return Ok(oTStaffTypeEnumDTOs);
        }

        //public ActionResult Upload()
        //{
        //    IFormFile file = Request.Form.Files[0];
        //    string folderName = "UploadExcel";
        //    string webRootPath = _hostingEnv.WebRootPath;
        //    string newPath = Path.Combine(webRootPath, folderName);
        //    StringBuilder sb = new StringBuilder();
        //    if (!Directory.Exists(newPath))
        //    {
        //        Directory.CreateDirectory(newPath);
        //    }
        //    if (file.Length > 0)
        //    {
        //        string sFileExtension = Path.GetExtension(file.FileName).ToLower();
        //        ISheet sheet;
        //        string fullPath = Path.Combine(newPath, file.FileName);
        //        using (var stream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            file.CopyTo(stream);
        //            stream.Position = 0;
        //            if (sFileExtension == ".xls")
        //            {
        //                HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
        //                sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
        //            }
        //            else
        //            {
        //                XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
        //                sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
        //            }
        //            IRow headerRow = sheet.GetRow(0); //Get Header Row
        //            int cellCount = headerRow.LastCellNum;
        //            sb.Append("<table class='table table-bordered'><tr>");
        //            for (int j = 0; j < cellCount; j++)
        //            {
        //                NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
        //                if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
        //                sb.Append("<th>" + cell.ToString() + "</th>");
        //            }
        //            sb.Append("</tr>");
        //            sb.AppendLine("<tr>");
        //            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
        //            {
        //                IRow row = sheet.GetRow(i);
        //                if (row == null) continue;
        //                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
        //                for (int j = row.FirstCellNum; j < cellCount; j++)
        //                {
        //                    if (row.GetCell(j) != null)
        //                        sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
        //                }
        //                sb.AppendLine("</tr>");
        //            }
        //            sb.Append("</table>");
        //        }
        //    }
        //    return this.Content(sb.ToString());
        //}

        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcelFile(IFormFile FormFile)
        {
            if (FormFile == null)
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
                    Staff staff = new Staff();
                    staff = await _userManager.AddUserAndTimestamp(staff, User, DbEnum.DbActionEnum.Create);
                    dt.Columns.Add("IsDelete", typeof(bool));
                    dt.Columns.Add("CreatedAt", typeof(DateTime));
                    dt.Columns.Add("CreatedBy", typeof(string));
                    dt.Columns.Add("UpdatedAt", typeof(DateTime));
                    dt.Columns.Add("UpdatedBy", typeof(string));
                    
                    dt.Columns.Add("BranchId", typeof(int));
                    foreach (var row in dt.AsEnumerable().ToList())
                    {
                        row["IsDelete"] = staff.IsDelete;
                        row["CreatedAt"] = staff.CreatedAt;
                        row["CreatedBy"] = staff.CreatedBy;
                        row["UpdatedAt"] = staff.UpdatedAt;
                        row["UpdatedBy"] = staff.UpdatedBy;
                        row["BranchId"] = staff.BranchId;
                    }
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.Staff";

                        // Map the Excel columns with that of the database table, this is optional but good if you do
                        // 
                        sqlBulkCopy.ColumnMappings.Add("CreatedAt", "CreatedAt");
                        sqlBulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                        sqlBulkCopy.ColumnMappings.Add("UpdatedAt", "UpdatedAt");
                        sqlBulkCopy.ColumnMappings.Add("UpdatedBy", "UpdatedBy");
                        sqlBulkCopy.ColumnMappings.Add("IsDelete", "IsDelete");
                        sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                        sqlBulkCopy.ColumnMappings.Add("NRC", "NRC");
                        sqlBulkCopy.ColumnMappings.Add("DOB", "DOB");
                        sqlBulkCopy.ColumnMappings.Add("Phone", "Phone");
                        sqlBulkCopy.ColumnMappings.Add("Age", "Age");
                        sqlBulkCopy.ColumnMappings.Add("Gender", "Gender");
                        sqlBulkCopy.ColumnMappings.Add("ProfileImage", "ProfileImage");
                        sqlBulkCopy.ColumnMappings.Add("Brief", "Brief");
                        sqlBulkCopy.ColumnMappings.Add("Department", "DepartmentId");
                        sqlBulkCopy.ColumnMappings.Add("Code", "Code");
                        sqlBulkCopy.ColumnMappings.Add("Position", "PositionId");
                        sqlBulkCopy.ColumnMappings.Add("BranchId", "BranchId");

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
