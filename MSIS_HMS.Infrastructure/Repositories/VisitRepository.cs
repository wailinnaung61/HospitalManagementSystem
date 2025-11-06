using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class VisitRepository : Repository<Visit>, IVisitRepository
    {
        public VisitRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Visit> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetVisits");
            var visittypes = ds.Tables[0].ToList<Visit>();
            return visittypes;
        }
        public override List<Visit> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetVisits", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var visittypes = ds.Tables[0].ToList<Visit>();
            return visittypes;
        }
        public List<Visit> GetAll(int? BranchId = null, int? VisitId = null, string VisitNo = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null, int? PatientId = null, int? DoctorId = null, int? VisitTypeId = null, VisitStatusEnum? Status = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetVisits", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "VisitId", VisitId },
                { "VisitNo", VisitNo },
                { "Date", Date },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
                { "PatientId", PatientId },
                { "DoctorId", DoctorId },
                { "VisitTypeId", VisitTypeId },
                { "Status", Status },
            });
            var visittypes = ds.Tables[0].ToList<Visit>();
            return visittypes;
        }
        public List<VisitPatientDTO> GetVisitPatient(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null, VisitStatusEnum? Status = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetVisitPatients", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
                { "Status", Status }
            });
            var visistPatients = ds.Tables[0].ToList<VisitPatientDTO>();
            return visistPatients;
        }
        public List<DoctorHistoryDTO> GetDoctorHistory(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null, VisitStatusEnum? Status = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDoctorHistory", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
                { "Status", Status }
            });
            var visistPatients = ds.Tables[0].ToList<DoctorHistoryDTO>();
            return visistPatients;
        }
        public override Visit Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetVisits", new Dictionary<string, object>() { { "VisitId", Id } });
            var visits = ds.Tables[0].ToList<Visit>();
            visits.ForEach(x => x.Doctor = GetDoctor(x.DoctorId));
            return visits.Count > 0 ? visits[0] : null;
        }
        public List<decimal> GetCFFee(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            List<decimal> decAmount = new List<decimal>();
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetCFFee",
                new Dictionary<string, object>()
                {   { "BranchId", BranchId },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
                });
            var cfFee = ds.Tables[0];
            if(!string.IsNullOrEmpty(cfFee.Rows[0][0].ToString()))
            {
                decAmount[0] = Convert.ToDecimal(cfFee.Rows[0][0]);

            }
            if(!string.IsNullOrEmpty(cfFee.Rows[0][1].ToString()))
            {
                decAmount[1] = Convert.ToDecimal(cfFee.Rows[0][1]);
            }
            return decAmount;
        }
        
        public List<CFFeeReportDTO> GetCFFeeReport(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null,int? DoctorId=null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetCFFeeReportPerDoctor",
                new Dictionary<string, object>()
                { { "BranchId", BranchId } ,
                    { "StartDate",StartDate},                 
                    {"EndDate",EndDate },
                     {"DoctorId",DoctorId }
                });
            var visistPatients = ds.Tables[0].ToList<CFFeeReportDTO>();
            return visistPatients;

        }

        private Doctor GetDoctor(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDoctors", new Dictionary<string, object>() { { "DoctorId", Id } });
            var doctors = ds.Tables[0].ToList<Doctor>();
            return doctors.Count > 0 ? doctors[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var visit = await _context.Visits.FindAsync(Id);
                    if (visit == null)
                    {
                        return false;
                    }
                    visit.IsDelete = true;
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
    }
}
