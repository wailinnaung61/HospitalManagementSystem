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
    public class AppointmentTypeRepository : Repository<AppointmentType>,IAppointmentTypeRepository
    {
        public AppointmentTypeRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
            
        }
        public override List<AppointmentType> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAppointmentTypes");
            var appointmenttypes = ds.Tables[0].ToList<AppointmentType>();
            return appointmenttypes;
        }
        public override List<AppointmentType> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAppointmentTypes", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var appointmenttypes = ds.Tables[0].ToList<AppointmentType>();
            return appointmenttypes;
        }
        public override AppointmentType Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAppointmentTypes", new Dictionary<string, object>() { { "AppointmentTypeId", Id } });
            var appointmentTypes = ds.Tables[0].ToList<AppointmentType>();
            return appointmentTypes.Count > 0 ? appointmentTypes[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var appointmentType = await _context.AppointmentTypes.FindAsync(Id);
                    if (appointmentType == null)
                    {
                        return false;
                    }
                    appointmentType.IsDelete = true;
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