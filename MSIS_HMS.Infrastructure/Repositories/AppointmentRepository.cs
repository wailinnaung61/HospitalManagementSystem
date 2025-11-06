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
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Appointment> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAppointments");
            var appointmenttypes = ds.Tables[0].ToList<Appointment>();
            return appointmenttypes;
        }
        public override List<Appointment> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAppointments", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var appointmenttypes = ds.Tables[0].ToList<Appointment>();
            return appointmenttypes;
        }
        public override Appointment Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAppointments", new Dictionary<string, object>()
            { { "AppointmentId", Id } });
            var appointments = ds.Tables[0].ToList<Appointment>();
            return appointments.Count > 0 ? appointments[0] : null;
        }
        public List<Appointment> GetAppointmentByDoctorId(int? DoctorId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAppointments", new Dictionary<string, object>()
            { { "DoctorId", DoctorId } });
            var appointments = ds.Tables[0].ToList<Appointment>();
            return appointments;
        }
        public List<Appointment> GetAppointmentByDateTime(DateTime startDate, DateTime endDate)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAppointments", new Dictionary<string, object>()
            {   { "StartDate", startDate },
                {"EndDate",endDate }
            });
            var appointments = ds.Tables[0].ToList<Appointment>();
            return appointments;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var appointment = await _context.Appointments.FindAsync(Id);
                    if (appointment == null)
                    {
                        return false;
                    }
                    appointment.IsDelete = true;
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