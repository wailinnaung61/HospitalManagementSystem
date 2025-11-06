using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace MSIS_HMS.Controllers
{
    public class DischargeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBranchService _branchService;
        private readonly ILogger<IPDRecordsController> _logger;
        private readonly Pagination _pagination;
        private readonly IPatientRepository _patientRepository;
        private readonly IIPDRecordRepository _iPDRecordRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IBedRepository _bedRepository;
        private readonly IWardRepository _wardRepository;
        private readonly IIPDAllotmentRepository _ipdallotmentRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserService _userService;

        public DischargeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IBranchService branchService, ILogger<IPDRecordsController> logger, IOptions<Pagination> pagination, IPatientRepository patientRepository, IIPDRecordRepository iPDRecordRepository, IRoomRepository roomRepository, IBedRepository bedRepository, IRoomTypeRepository roomTypeRepository, IWardRepository wardRepository, IIPDAllotmentRepository ipdallotmentRepository, IDepartmentRepository departmentRepository,IUserService userService)
        {
            _userManager = userManager;
            _context = context;
            _branchService = branchService;
            _logger = logger;
            _pagination = pagination.Value;
            _patientRepository = patientRepository;
            _ipdallotmentRepository = ipdallotmentRepository;
            _roomRepository = roomRepository;
            _bedRepository = bedRepository;
            _wardRepository = wardRepository;
            _iPDRecordRepository = iPDRecordRepository;
            _departmentRepository = departmentRepository;
            _userService = userService;
        }
        public void Initialize()
        {
            var branch = _branchService.GetBranchByUser();
            var patients = _context.Patients.Where(x => x.IsDelete == false).ToList();
            var rooms = _context.Rooms.Where(x => x.IsDelete == false).ToList();
            var beds = _context.Beds.Where(x => x.IsDelete == false).ToList();
            var departments = _context.Departments.Where(x => x.IsDelete == false && x.Type == Core.Enums.DepartmentTypeEnum.IPD);
            ViewData["Patients"] = new SelectList(patients, "Id", "Name");
            ViewData["Departments"] = new SelectList(departments, "Id", "Name");
            ViewData["Rooms"] = new SelectList(rooms, "Id", "RoomNo");
            ViewData["Beds"] = new SelectList(beds, "Id", "No");
            ViewData["CheckIn"] = branch.CheckInTime;
            ViewData["CheckOut"] = branch.CheckOutTime;
        }
        public IActionResult Index(int? page = 1, int? IPDRecordId = null, string Status = null, int? PaymentType = null, int? RoomId = null, int? BedId = null, string VoucherNo = null)
        {
            Initialize();
            var user = _userService.Get(User);
            var ipdrecords = _iPDRecordRepository.GetAll(user.BranchId,null, Status, PaymentType, BedId, RoomId, VoucherNo,(int)IPDStatusEnum.Discharged,null);
            var rooms = _roomRepository.GetAll();
            var beds = _bedRepository.GetAll();
            var patients = _patientRepository.GetAll();
            var departments = _departmentRepository.GetAll();
            ipdrecords.ForEach(x => x.Patient = patients.SingleOrDefault(b => b.Id == x.PatientId));
            ipdrecords.ForEach(x => x.Room = rooms.SingleOrDefault(b => b.Id == x.RoomId));
            ipdrecords.ForEach(x => x.Bed = beds.SingleOrDefault(b => b.Id == x.BedId));
            ipdrecords.ForEach(x => x.Department = departments.SingleOrDefault(b => b.Id == x.DepartmentId));
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            return View(ipdrecords.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create(int ipdRecordId)
        {
            Initialize();
            IPDRecord iPDRecord = new IPDRecord();
            iPDRecord.Id = ipdRecordId;
            return View(iPDRecord);
        }
        [HttpPost]
        public async Task<IActionResult> Create(IPDRecord ipdrecord)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    
                        var recordData = _iPDRecordRepository.Get(ipdrecord.Id);
                        recordData.DODC = ipdrecord.DODC;
                        recordData.CheckOutTime = ipdrecord.CheckOutTime;
                        recordData.ResasonofDODC = ipdrecord.ResasonofDODC;
                        recordData.IPDStatusEnum =IPDStatusEnum.Discharged;
                        await ChangeRoomandBedStatus(recordData, "Available");

                        recordData = await _userManager.AddUserAndTimestamp(recordData, User, DbEnum.DbActionEnum.Update);
                        var _ipdrecord = await _iPDRecordRepository.UpdateAsync(recordData);
                        if (_ipdrecord != null)
                        {
                            //IPDAllotment ipdallotment = new IPDAllotment();
                            var ipdallotment = _ipdallotmentRepository.GetAll(_ipdrecord.Id).FirstOrDefault();
                            ipdallotment = await _userManager.AddUserAndTimestamp(ipdallotment, User, DbEnum.DbActionEnum.Update);
                            ipdallotment.CheckOutTime = _ipdrecord.CheckOutTime;
                           
                            ipdallotment.IPDRecordId = _ipdrecord.Id;
                           
                            await _ipdallotmentRepository.UpdateAsync(ipdallotment);
                            await _branchService.IncreaseVoucherNo(VoucherTypeEnum.IPD);
                            await transaction.CommitAsync();
                            TempData["notice"] = StatusEnum.NoticeStatus.Success;
                            _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                            return RedirectToAction(nameof(Index));
                        }
                    
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                }

            }
            Initialize();
            return View();
        }
        private async Task ChangeRoomandBedStatus(IPDRecord iPDRecord, string Status)
        {
            if (iPDRecord.RoomId != null && iPDRecord.BedId != null)
            {

                var bed = await _context.Beds.FindAsync(iPDRecord.BedId);

                bed.Status = Status;
                await _context.SaveChangesAsync();
            }
            else if (iPDRecord.RoomId != null && iPDRecord.BedId == null)
            {
                var room = await _context.Rooms.FindAsync(iPDRecord.RoomId);
                room.Status = Status;
                await _context.SaveChangesAsync();

                var bed = _context.Beds.Where(x => x.RoomId == iPDRecord.RoomId).ToList();
                foreach (var b in bed)
                {
                    b.Status = Status;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
