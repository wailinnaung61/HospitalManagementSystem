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

namespace MSIS_HMS.Infrastructure.Repositories.Base
{
    public class ServiceTypeRepository : Repository<ServiceType> , IServiceTypeRepository
    {
        public ServiceTypeRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override List<ServiceType> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetServiceTypes");
            var serviceTypes = ds.Tables[0].ToList<ServiceType>();
            return serviceTypes;
        }
        public override List<ServiceType> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetServiceTypes", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var serviceTypes = ds.Tables[0].ToList<ServiceType>();
            return serviceTypes;
        }
        public override ServiceType Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetServiceTypes", new Dictionary<string, object>() { { "ServiceTypeId", Id } });
            var serviceTypes = ds.Tables[0].ToList<ServiceType>();
            return serviceTypes.Count > 0 ? serviceTypes[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var serviceTypes = await _context.ServiceTypes.FindAsync(Id);
                    if (serviceTypes == null)
                    {
                        return false;
                    }
                    serviceTypes.IsDelete = true;
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