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
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext context, IConfigService config) : base(context, config)
        {
        }
        public override List<Doctor> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDoctors");
            var doctors = ds.Tables[0].ToList<Doctor>();
            return doctors;
        }
        public override List<Doctor> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDoctors", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var doctors = ds.Tables[0].ToList<Doctor>();
            return doctors;
        }
        public List<Doctor> GetAll(int? BranchId = null, int? DoctorId = null, string Name = null, string Code = null, string SamaNo = null, int? DepartmentId = null, int? DepartmentType = null, int? SpecialityId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDoctors", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "DoctorId", DoctorId },
                { "Name", Name },
                { "Code", Code },
                { "SamaNo", SamaNo },
                { "DepartmentId", DepartmentId },
                { "SpecialityId", SpecialityId },
            });
            var doctors = ds.Tables[0].ToList<Doctor>();
            return doctors;
        }
        public override Doctor Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDoctors", new Dictionary<string, object>() { { "DoctorId", Id } });
            var doctors = ds.Tables[0].ToList<Doctor>();
            doctors.ForEach(x => x.Schedules = GetSchedules(x.BranchId, x.Id));
            return doctors.Count > 0 ? doctors[0] : null;
        }
        public List<Schedule> GetSchedules(int? BranchId, int? DoctorId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetSchedules", new Dictionary<string, object>() { { "BranchId", BranchId }, { "DoctorId", DoctorId } });
            var schedules = ds.Tables[0].ToList<Schedule>();
            schedules.ForEach(x => x.DayOfWeekName = x.DayOfWeek.ToString());
            return schedules;
        }
        public override async Task<Doctor> UpdateAsync(Doctor doctor)
        {
            if (doctor != null)
            {
                doctor.Schedules.ToList().ForEach(x =>
                {
                    x.DoctorId = doctor.Id;
                    x.BranchId = doctor.BranchId;
                });
                var existingDoctor = _context.Doctors
                    .Where(p => p.Id == doctor.Id)
                    .Include(p => p.Schedules)
                    .SingleOrDefault();

                if (existingDoctor != null)
                {
                    // Update parent
                    _context.Entry(existingDoctor).CurrentValues.SetValues(doctor);

                    // Delete children
                    _context.Schedules.RemoveRange(existingDoctor.Schedules);

                    // Insert children
                    existingDoctor.Schedules = doctor.Schedules;

                    await _context.SaveChangesAsync();
                    return doctor;
                }
            }
            return null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var doctor = await _context.Doctors.FindAsync(Id);
                    if (doctor == null)
                    {
                        return false;
                    }
                    doctor.IsDelete = true;
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