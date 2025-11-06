using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Helpers;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace MSIS_HMS.Controllers
{
    public class IPDRecordsController : Controller
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
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IWardRepository _wardRepository;
        private readonly IIPDAllotmentRepository _ipdallotmentRepository;
        private readonly IUserService _userService;
        private readonly IDepartmentRepository _departmentRepository;

        public IPDRecordsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IBranchService branchService, ILogger<IPDRecordsController> logger, IOptions<Pagination> pagination, IPatientRepository patientRepository, IIPDRecordRepository iPDRecordRepository, IRoomRepository roomRepository, IBedRepository bedRepository, IRoomTypeRepository roomTypeRepository, IWardRepository wardRepository, IIPDAllotmentRepository ipdallotmentRepository, IDepartmentRepository departmentRepository,IUserService userService)
        {
            _context = context;
            _userManager = userManager;
            _branchService = branchService;
            _logger = logger;
            _pagination = pagination.Value;
            _patientRepository = patientRepository;
            _iPDRecordRepository = iPDRecordRepository;
            _roomRepository = roomRepository;
            _bedRepository = bedRepository;
            _roomTypeRepository = roomTypeRepository;
            _wardRepository = wardRepository;
            _ipdallotmentRepository = ipdallotmentRepository;
            _departmentRepository = departmentRepository;
            _userService = userService;
        }

        public void Initialize(IPDRecord iPDRecord=null)
        {
            var branch = _branchService.GetBranchByUser();
            var patients = _context.Patients.Where(x => x.IsDelete == false).ToList();
            var rooms = _context.Rooms.Where(x => x.IsDelete == false).ToList();
            var beds = _context.Beds.Where(x => x.IsDelete == false).ToList();
            var departments = _context.Departments.Where(x => x.IsDelete == false && x.Type == Core.Enums.DepartmentTypeEnum.IPD);
            ViewData["Patients"] = new SelectList(patients, "Id", "Name",iPDRecord?.PatientId);
            ViewData["Departments"] = new SelectList(departments, "Id", "Name");
            ViewData["Rooms"] = new SelectList(rooms, "Id", "RoomNo");
            ViewData["Beds"] = new SelectList(beds, "Id", "No");
            ViewData["UseVoucherFormat"] = branch.UseVoucherFormatForIPD;
            ViewData["CheckIn"] = branch.CheckInTime;
            ViewData["CheckOut"] = branch.CheckOutTime;
        }
        public IActionResult GetAvailableRoomsandBeds(int departmentId)
        {
            var branch = _branchService.GetBranchByUser();
            if (branch != null)
            {
                var avaliableRoomsandBeds = _iPDRecordRepository.GetAvailableRoomsandBeds(departmentId);
                foreach (var r in avaliableRoomsandBeds)
                {
                    var beds = _context.Beds.Where(x => x.RoomId == r.Id && x.Status == "Available" && x.IsDelete == false).ToList();
                    r.Beds = beds;
                }
                var roomtypes = _roomTypeRepository.GetAll();
                var wards = _wardRepository.GetAll();
                return Ok(avaliableRoomsandBeds);
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
        public IActionResult ChangeRoom(int ipdRecordId, int? allotmentId)
        {
            if (allotmentId == null)
            {
                var allotment = _ipdallotmentRepository.GetIPDRecord(ipdRecordId);
                allotment.IPDRecord = _iPDRecordRepository.Get(ipdRecordId);
                allotment.FromRoomId = allotment.IPDRecord.RoomId;
                allotment.FromBedId = allotment.IPDRecord.BedId;
                allotment.ToRoomId = 0;
                allotment.ToBedId = null;
                allotment.Reason = "";
                ViewData["Action"] = "ChangeRoom";
                Initialize();
                allotment.IPDRecord.Image = allotment.IPDRecord.Image.GetBase64();
                return View(allotment);
            }
            else
            {
                var allotment = _ipdallotmentRepository.GetIPDAllotment(allotmentId);
                allotment.IPDRecord = _iPDRecordRepository.Get(ipdRecordId);
                allotment.NewCheckInTime = allotment.CheckInTime;
                allotment.NewCheckOutTime = allotment.CheckOutTime;
                ViewData["Action"] = "EditChangeRoom";
                Initialize();
                allotment.IPDRecord.Image = allotment.IPDRecord.Image.GetBase64();
                return View(allotment);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRoom(IPDAllotment allotment)
        {
            if (Request.Form["ChangeRoom"] == "ChangeRoom")
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        allotment.IPDRecord = _iPDRecordRepository.GetIPDSingleRecord(allotment.IPDRecordId);

                        //for old bed or room
                        await ChangeRoomandBedStatus(allotment.IPDRecord, "Available");

                        //update ipdrecord
                        allotment.IPDRecord.RoomId = allotment.ToRoomId;
                        allotment.IPDRecord.BedId = allotment.ToBedId;
                        var _ipdrecord = await _iPDRecordRepository.UpdateAsync(allotment.IPDRecord);

                        //for new bed or room
                        await ChangeRoomandBedStatus(_ipdrecord, "Not Available");

                        //update ipdallotment checkout time
                        var roomCheckout = new IPDAllotment { Id = allotment.Id, CheckOutTime = allotment.CheckOutTime };
                        _context.IPDAllotments.Attach(roomCheckout).Property(x => x.CheckOutTime).IsModified = true;
                        _context.SaveChanges();

                        //insert ipdallotment 
                        var _allotment = await _userManager.AddUserAndTimestamp(allotment, User, DbEnum.DbActionEnum.Create);
                        _allotment.Id = 0;
                        _allotment.CheckInTime = allotment.NewCheckInTime;
                        _allotment.CheckOutTime = allotment.NewCheckOutTime;
                        await _ipdallotmentRepository.AddAsync(_allotment);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.InnerException.Message);
                        await transaction.RollbackAsync();
                    }
                    await transaction.CommitAsync();
                    TempData["notice"] = StatusEnum.NoticeStatus.Success;
                    return RedirectToAction("BedTransferHistory", "IPDRecords", new { id=allotment.IPDRecordId});
                }
            }
            else if(Request.Form["EditChangeRoom"] == "EditChangeRoom")
            {
                var _allotment = await _userManager.AddUserAndTimestamp(allotment, User, DbEnum.DbActionEnum.Update);
                _allotment.CheckInTime = _allotment.NewCheckInTime;
                _allotment.CheckOutTime = _allotment.NewCheckOutTime;
                await _ipdallotmentRepository.UpdateAsync(_allotment);
                TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                return RedirectToAction("BedTransferHistory", "IPDRecords", new { id = allotment.IPDRecordId });
            }
            return BadRequest();
        }

        public IActionResult Index(int? page = 1, int? IPDRecordId = null, string Status = null, int? PaymentType = null, int? RoomId = null, int? BedId = null, string VoucherNo = null)
        {
            Initialize();
            var user = _userService.Get(User);
            var ipdrecords = _iPDRecordRepository.GetAll(user.BranchId,null, Status, PaymentType, BedId, RoomId, VoucherNo);
            var rooms = _roomRepository.GetAll();
            var beds = _bedRepository.GetAll();
            var patients=_patientRepository.GetAll();
            var departments = _departmentRepository.GetAll();
            ipdrecords.ForEach(x => x.Patient = patients.SingleOrDefault(b => b.Id == x.PatientId));
            ipdrecords.ForEach(x => x.Room = rooms.SingleOrDefault(b => b.Id == x.RoomId));
            ipdrecords.ForEach(x => x.Bed = beds.SingleOrDefault(b => b.Id == x.BedId));
            ipdrecords.ForEach(x=>x.Department=departments.SingleOrDefault(b=>b.Id == x.DepartmentId));
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            return View(ipdrecords.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }
        public IActionResult Create(int? patientId=null)
        {

            var ipdrecord = new IPDRecord
            {
                VoucherNo = _branchService.GetVoucherNo(VoucherTypeEnum.IPD),
                PatientId = patientId==null?0:Convert.ToInt32(patientId)
            };
            Initialize(ipdrecord);
            return View(ipdrecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IPDRecord ipdrecord)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //to check draft or admission
                        if(ipdrecord.RoomId == null)
                        {
                            ipdrecord.IPDStatusEnum = Core.Enums.IPDStatusEnum.Draft;
                        }
                        else
                        {
                            ipdrecord.IPDStatusEnum = Core.Enums.IPDStatusEnum.Admission;
                        }

                        //voucher and paid
                        var branch = _branchService.GetBranchByUser();
                        ipdrecord.VoucherNo = _branchService.GetVoucherNo(VoucherTypeEnum.IPD, ipdrecord.VoucherNo);
                        if (string.IsNullOrEmpty(ipdrecord.VoucherNo))
                        {
                            ModelState.AddModelError("VoucherNo", "This field is required.");
                            Initialize();
                            return View(ipdrecord);
                        }                        
                        if (branch.AutoPaidForOrder)
                        {
                            ipdrecord.IsPaid = true;
                            ipdrecord.PaidDate = DateTime.Now;
                        }

                        //change available not available
                        await ChangeRoomandBedStatus(ipdrecord, "Not Available");

                        //save to ipdrecord
                        ipdrecord = await _userManager.AddUserAndTimestamp(ipdrecord, User, DbEnum.DbActionEnum.Create);
                        var _ipdrecord = await _iPDRecordRepository.AddAsync(ipdrecord);
                        
                        //save to allotment
                        if (_ipdrecord != null && _ipdrecord.RoomId != null)
                        {
                            IPDAllotment ipdallotment = new IPDAllotment();
                            ipdallotment = await _userManager.AddUserAndTimestamp(ipdallotment, User, DbEnum.DbActionEnum.Create);
                            ipdallotment.Date = _ipdrecord.DOA;
                            if (_ipdrecord.RoomId != null)
                            {
                                ipdallotment.ToRoomId = (int)_ipdrecord.RoomId;
                            }
                            if (ipdrecord.BedId != null)
                            {
                                ipdallotment.ToBedId = _ipdrecord.BedId;
                            }
                            ipdallotment.IPDRecordId = _ipdrecord.Id;
                            ipdallotment.CheckInTime = _ipdrecord.CheckInTime;
                            //ipdallotment.CheckOutTime = _ipdrecord.CheckOutTime;
                            await _ipdallotmentRepository.AddAsync(ipdallotment);
                            await _branchService.IncreaseVoucherNo(VoucherTypeEnum.IPD);
                            await transaction.CommitAsync();
                            TempData["notice"] = StatusEnum.NoticeStatus.Success;
                            _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                            return RedirectToAction(nameof(Index));
                        }

                        //draft only save in ipdrecord
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
            return View(ipdrecord);
        }

        public IActionResult Edit(int id)
        {
            var ipdrecord = _iPDRecordRepository.Get(id);
            Initialize();
            return View(ipdrecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IPDRecord ipdrecord)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //to check draft or admission
                    if (ipdrecord.RoomId == null)
                    {
                        ipdrecord.IPDStatusEnum = Core.Enums.IPDStatusEnum.Draft;
                    }
                    else
                    {
                        ipdrecord.IPDStatusEnum = Core.Enums.IPDStatusEnum.Admission;
                        await ChangeRoomandBedStatus(ipdrecord, "Not Available");
                        if (ipdrecord != null && ipdrecord.RoomId != null)
                        {
                            IPDAllotment ipdallotment = new IPDAllotment();
                            ipdallotment = await _userManager.AddUserAndTimestamp(ipdallotment, User, DbEnum.DbActionEnum.Create);
                            ipdallotment.Date = ipdrecord.DOA;
                            if (ipdrecord.RoomId != null)
                            {
                                ipdallotment.ToRoomId = (int)ipdrecord.RoomId;
                            }
                            if (ipdrecord.BedId != null)
                            {
                                ipdallotment.ToBedId = ipdrecord.BedId;
                            }
                            ipdallotment.IPDRecordId = ipdrecord.Id;
                            ipdallotment.CheckInTime = ipdrecord.CheckInTime;
                            await _ipdallotmentRepository.AddAsync(ipdallotment);
                        }
                    }
                    ipdrecord = await _userManager.AddUserAndTimestamp(ipdrecord, User, DbEnum.DbActionEnum.Update);
                    var _ipdrecord = await _iPDRecordRepository.UpdateAsync(ipdrecord);
                    if (_ipdrecord != null)
                    {
                        TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                        _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            Initialize();
            return View(ipdrecord);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ipdoldrecord = _iPDRecordRepository.Get(id);
                var _itemType = await _iPDRecordRepository.DeleteAsync(id);
                if (_itemType == false)
                {
                    await ChangeRoomandBedStatus(ipdoldrecord, "Available");
                }
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult BedTransferHistory(int id)
        {
            var bedTransferHistory = _ipdallotmentRepository.GetAll(id).OrderByDescending(x => x.Date);
            ViewData["Histories"] = bedTransferHistory.OrderByDescending(x => x.UpdatedAt).ToList();
            var iPDRecord = _iPDRecordRepository.Get(id);
            iPDRecord.Image = iPDRecord.Image.GetBase64();
            return View(iPDRecord);
        }

        public IActionResult Details(int id, DateTime? selectedDate)
        {
            var iPDRecord = _iPDRecordRepository.Get(id);
            if (iPDRecord == null)
            {
                return BadRequest();
            }
            iPDRecord.Image = iPDRecord.Image.GetBase64();
            iPDRecord.SelectedDate = selectedDate == null ? DateTime.Now.Date.ToString("yyyy-MM-dd") : ((DateTime)selectedDate).ToString("yyyy-MM-dd");
            iPDRecord.OutletId = iPDRecord.Room.Ward.OutletId;
            return View(iPDRecord);
        }

        #region IPDOrderService
        public async Task<IActionResult> GetIPDOrderServices(int id, DateTime date)
        {
            var orderServices = await _context.IPDOrderServices.Include(x => x.Service).Where(x => x.IPDRecordId == id && x.Date.Date == date.Date).ToListAsync();
            orderServices.ForEach(x => x.ServiceName = x.Service.Name);
            return Ok(orderServices);
        }

        public async Task<IActionResult> AddOrderService(IPDOrderService orderService)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.IPDOrderServices.Add(orderService);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(orderService);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        public async Task<IActionResult> UpdateOrderService(IPDOrderService orderService)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Entry(orderService).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(orderService);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        public async Task<IActionResult> DeleteOrderService(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var orderService = _context.IPDOrderServices.Find(id);
                    _context.IPDOrderServices.Remove(orderService);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }
        #endregion

        #region IPDRound
        public async Task<IActionResult> GetIPDRounds(int id, DateTime date)
        {
            var rounds = await _context.IPDRounds.Include(x => x.Doctor).Where(x => x.IPDRecordId == id && x.Date.Date == date.Date).ToListAsync();
            rounds.ForEach(x => x.DoctorName = x.Doctor.Name);
            return Ok(rounds);
        }

        public async Task<IActionResult> AddRound(IPDRound round)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.IPDRounds.Add(round);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(round);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        public async Task<IActionResult> UpdateRound(IPDRound round)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Entry(round).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(round);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        public async Task<IActionResult> DeleteRound(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var round = _context.IPDRounds.Find(id);
                    _context.IPDRounds.Remove(round);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }
        #endregion

        #region IPDStaff
        public async Task<IActionResult> GetIPDStaffs(int id, DateTime date)
        {
            var staffs = await _context.IPDStaffs.Include(x => x.Staff).Where(x => x.IPDRecordId == id && x.Date.Date == date.Date).ToListAsync();
            staffs.ForEach(x => x.StaffName = x.Staff.Name);
            return Ok(staffs);
        }

        public async Task<IActionResult> AddStaff(IPDStaff model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.IPDStaffs.Add(model);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(model);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        public async Task<IActionResult> UpdateStaff(IPDStaff model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Entry(model).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(model);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        public async Task<IActionResult> DeleteStaff(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var staff = _context.IPDStaffs.Find(id);
                    _context.IPDStaffs.Remove(staff);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }
        #endregion

        #region IPDFood
        public async Task<IActionResult> GetIPDFoods(int id, DateTime date)
        {
            var foods = await _context.IPDFoods.Include(x => x.Food).Where(x => x.IPDRecordId == id && x.Date.Date == date.Date).ToListAsync();
            foods.ForEach(x => x.FoodName = x.Food.Name);
            return Ok(foods);
        }

        public async Task<IActionResult> AddFood(IPDFood model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.IPDFoods.Add(model);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(model);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        public async Task<IActionResult> UpdateFood(IPDFood model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Entry(model).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(model);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        public async Task<IActionResult> DeleteFood(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var food = _context.IPDFoods.Find(id);
                    _context.IPDFoods.Remove(food);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }
        #endregion

        #region IPDOrderItem
        public async Task<IActionResult> GetIPDOrderItems(int id, DateTime date)
        {
            var foods = await _context.IPDOrderItems
                .Include(x => x.Item)
                .Include(x => x.Unit)
                .Include(x => x.Outlet)
                .Where(x => x.IPDRecordId == id && x.Date.Date == date.Date)
                .ToListAsync();
            foods.ForEach(x =>
            {
                x.ItemName = x.Item.Name;
                x.UnitName = x.Unit.Name;
                x.OutletName = x.Outlet.Name;
            });
            return Ok(foods);
        }

        public async Task<IActionResult> AddOrderItem(IPDOrderItem model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.IPDOrderItems.Add(model);
                    var item = await _context.OutletItems.Where(x => x.ItemId == model.ItemId && x.OutletId == model.OutletId).FirstOrDefaultAsync();
                    item.Qty -= model.QtyInSmallestUnit;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(model);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        public async Task<IActionResult> UpdateOrderItem(IPDOrderItem model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var oldIPDOrderItem = _context.IPDOrderItems.Find(model.Id);
                    var oldQty = oldIPDOrderItem.QtyInSmallestUnit;
                    var oldOutletId = oldIPDOrderItem.OutletId;

                    var item = await _context.OutletItems.Where(x => x.ItemId == model.ItemId && x.OutletId == model.OutletId).FirstOrDefaultAsync();
                    if (oldOutletId == model.OutletId)
                    {
                        item.Qty += oldQty - model.QtyInSmallestUnit;
                    }
                    else
                    {
                        var oldItem = await _context.OutletItems.Where(x => x.ItemId == oldIPDOrderItem.ItemId && x.OutletId == oldIPDOrderItem.OutletId).FirstOrDefaultAsync();
                        oldItem.Qty += oldQty;
                        item.Qty -= model.QtyInSmallestUnit;
                    }

                    oldIPDOrderItem.Date = model.Date;
                    oldIPDOrderItem.OutletId = model.OutletId;
                    oldIPDOrderItem.ItemId = model.ItemId;
                    oldIPDOrderItem.UnitId = model.UnitId;
                    oldIPDOrderItem.UnitPrice = model.UnitPrice;
                    oldIPDOrderItem.Qty = model.Qty;
                    oldIPDOrderItem.QtyInSmallestUnit = model.QtyInSmallestUnit;
                    oldIPDOrderItem.IsFOC = model.IsFOC;
                    oldIPDOrderItem.SortOrder = model.SortOrder;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(model);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }

        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var model = _context.IPDOrderItems.Find(id);
                var item = await _context.OutletItems.Where(x => x.ItemId == model.ItemId && x.OutletId == model.OutletId).FirstOrDefaultAsync();
                item.Qty += model.QtyInSmallestUnit;
                _context.IPDOrderItems.Remove(model);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        public IActionResult IPDRecordDetail(int id)
        {
            var ipdrecord = _iPDRecordRepository.Get(id);

            Initialize();
            return View(ipdrecord);
        }
    }
}
