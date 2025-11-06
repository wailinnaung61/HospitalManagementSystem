using Microsoft.EntityFrameworkCore;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Core.Entities.DTOs;

namespace MSIS_HMS.Infrastructure.Repositories
{

    public class IPDRecordRepository : Repository<IPDRecord>, IIPDRecordRepository
    {
        public IPDRecordRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override IPDRecord Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetIPDRecords", new Dictionary<string, object>() { { "IPDRecordId", Id } });
            var iPDRecords = ds.Tables[0].ToList<IPDRecord>();
            var iPDRecord = iPDRecords.Count > 0 ? iPDRecords[0] : null;
            if (iPDRecord != null)
            {
                iPDRecord.Patient = GetPatient(iPDRecord.PatientId);
                var roomId = iPDRecord.RoomId ?? default(int);
                iPDRecord.Room = GetRoom(Convert.ToInt32(roomId));
                if (iPDRecord.BedId != null)
                {
                    var bedId = iPDRecord.BedId ?? default(int);
                    iPDRecord.Bed = GetBed(bedId);
                }
                iPDRecord.Department=GetDepartment(iPDRecord.DepartmentId);
            }
            return iPDRecord;
        }
        public IPDRecord GetIPDSingleRecord(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetIPDRecords", new Dictionary<string, object>() { { "IPDRecordId", Id } });
            var iPDRecords = ds.Tables[0].ToList<IPDRecord>();
            var iPDRecord = iPDRecords.Count > 0 ? iPDRecords[0] : null;           
            return iPDRecord;
        }
        public Patient GetPatient(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPatients", new Dictionary<string, object>() { { "PatientId", Id } });
            var patients= ds.Tables[0].ToList<Patient>();
            var patient = patients.Count > 0 ? patients[0] : null;
            return patient;
        }
        public Department GetDepartment(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDepartments", new Dictionary<string, object>() { { "DepartmentId", Id } });
            var departments = ds.Tables[0].ToList<Department>();
            var department = departments.Count > 0 ? departments[0] : null;
            return department;
        }
        public Bed GetBed(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBeds", new Dictionary<string, object>() { { "BedId", Id } });
            var beds = ds.Tables[0].ToList<Bed>();
            var bed = beds.Count > 0 ? beds[0] : null;
            return bed;
        }
        public Room GetRoom(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetRooms", new Dictionary<string, object>() { { "RoomId", Id } });
            var rooms = ds.Tables[0].ToList<Room>();
            var room = rooms.Count > 0 ? rooms[0] : null;
            if(room != null)
            {
                DataSet ds1 = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWards", new Dictionary<string, object>() { { "WardId", room.WardId } });
                var wards = ds1.Tables[0].ToList<Ward>();
                room.Ward= wards.Count > 0 ? wards[0] : null;
                if(room.Ward != null)
                {
                    DataSet ds2 = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetFloor", new Dictionary<string, object>() { { "FloorId", room.Ward.FloorId } });
                    var floors = ds2.Tables[0].ToList<Floor>();
                    room.Ward.Floor= floors.Count > 0 ? floors[0] : null;
                }
            }
            return room;
        }

        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var iPDRecord = await _context.IPDRecords.FindAsync(Id);
                    if (iPDRecord == null)
                    {
                        return false;
                    }
                    iPDRecord.IsDelete = true;

                    if (iPDRecord.IsDelete == true)
                    {
                        var iPDAllotment =  _context.IPDAllotments.Where(x => x.IPDRecordId == Id).ToList();
                        foreach (var allotment in iPDAllotment)
                        {
                            allotment.IsDelete = true;
                        }
                        
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (DbException e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                }
            }
            return false;
        }
        public List<IPDRecord> GetAll(int? BranchId=null,int? IPDRecordId=null, string Status=null, int? PaymentType=null, int? BedId=null, int? RoomId=null, string VoucherNo=null,int? TreatmentProcess=null,DateTime? DOA=null,DateTime? DODC=null,DateTime? StartDate=null,DateTime? EndDate=null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetIPDRecords", new Dictionary<string, object> {
                {"BranchId",BranchId},
                {"IPDRecordId",IPDRecordId },
                {"Status",Status },
                {"PaymentType",PaymentType },
                {"BedId",BedId },
                {"RoomId",RoomId },
                {"VoucherNo",VoucherNo },
                {"TreatmentProcess",TreatmentProcess },
                {"DOA" ,DOA},
                {"Dodc",DODC },
                { "StartDate",StartDate},
                {"EndDate",EndDate }
            });
            var ipdrecords = ds.Tables[0].ToList<IPDRecord>();
            return ipdrecords;
        }

        //public decimal CalculatePayment(int id)
        //{
        //    var total = 0M;
        //    var iPDRecord = _context.IPDRecords.Where(x => x.Id == id).Include(x => x.IPDAllotments)..Include(x => x.IPDOrderItems).Include(x => x.IPDOrderServices).Include(x => x.IPDRounds).Include(x => x.IPDStaffs).Include(x => x.IPDFoods).FirstOrDefault();
        //    if (iPDRecord != null)
        //    {
        //        var roomCharges = 0M;
        //        foreach (var room in iPDRecord.IPDAllotments)
        //        {
        //            room.
        //        }
        //    }
        //    return total;
        //}      

        List<Room> IIPDRecordRepository.GetAvailableRoomsandBeds(int DepartmentId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAvailableRoomsandBeds", new Dictionary<string, object> {
                {"DepartmentId",DepartmentId }
            });
            var rooms = ds.Tables[0].ToList<Room>();
            return rooms;
        }
        public decimal GetIncomeForIPD(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            decimal dailyTotalAmt = 0;
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetIncomeForIPD", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "StartDate", StartDate },
                { "EndDate", EndDate }
            });
            var incomeAmt = ds.Tables[0].ToList<DailyIncomeDTO>();
            foreach (var amt in incomeAmt)
            {
                dailyTotalAmt =  amt.IPDIncome;
            }
            return dailyTotalAmt;
        }
        public List<IPDRecordForDashboardDTO> GetIPDRecordForDashboard(int? BranchId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetIPDRecordForDashboard", new Dictionary<string, object>() {
                { "BranchId", BranchId }

            });

            var admittedPatient = ds.Tables[0].ToList<IPDRecordForDashboardDTO>();
           
            return admittedPatient;
        }

    }
}


