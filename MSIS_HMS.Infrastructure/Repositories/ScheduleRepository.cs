using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Schedule> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetSchedules");
            var schedules = ds.Tables[0].ToList<Schedule>();
            schedules.ForEach(x => x.DayOfWeekName = x.DayOfWeek.ToString());
            return schedules;
        }
        public override List<Schedule> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetSchedules", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var schedules = ds.Tables[0].ToList<Schedule>();
            schedules.ForEach(x => x.DayOfWeekName = x.DayOfWeek.ToString());
            return schedules;
        }
        public List<Schedule> GetAll(int? BranchId, int? DoctorId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetSchedules", new Dictionary<string, object>() { { "BranchId", BranchId }, { "DoctorId", DoctorId } });
            var schedules = ds.Tables[0].ToList<Schedule>();
            schedules.ForEach(x => x.DayOfWeekName = x.DayOfWeek.ToString());
            return schedules;
        }
        public List<Schedule> GetAll(int? BranchId = null, int? ScheduleId = null, int? DepartmentId = null, int? DepartmentType = null, int? DoctorId = null, int? DayOfWeek = null, TimeSpan? Time = null, TimeSpan? FromTime = null, TimeSpan? ToTime = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetSchedules", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "ScheduleId", ScheduleId },
                { "DepartmentId", DepartmentId },
                { "DepartmentType", DepartmentType },
                { "DoctorId", DoctorId },
                { "DayOfWeek", DayOfWeek },
                { "Time", Time },
                { "FromTime", FromTime },
                { "ToTime", ToTime }
            });
            var schedules = ds.Tables[0].ToList<Schedule>();
            schedules.ForEach(x => x.DayOfWeekName = x.DayOfWeek.ToString());
            return schedules;
        }
        public override Schedule Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetSchedules", new Dictionary<string, object>() { { "ScheduleId", Id } });
            var schedules = ds.Tables[0].ToList<Schedule>();
            schedules.ForEach(x => x.DayOfWeekName = x.DayOfWeek.ToString());
            return schedules.Count > 0 ? schedules[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var visitType = await _context.Schedules.FindAsync(Id);
                    if (visitType == null)
                    {
                        return false;
                    }
                    visitType.IsDelete = true;
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