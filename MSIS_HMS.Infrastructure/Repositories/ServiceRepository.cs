using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class ServiceRepository : Repository<Service> , IServiceRepository
    {
        public ServiceRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override List<Service> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetServices");
            var services = ds.Tables[0].ToList<Service>();
            return services;
        }
        public override List<Service> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetServices", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var services = ds.Tables[0].ToList<Service>();
            return services;
        }
        public override Service Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetServices", new Dictionary<string, object>() { { "ServiceId", Id }});
            var services = ds.Tables[0].ToList<Service>();
            return services.Count > 0 ? services[0] : null;
        }
        public List<ServiceDTO> GetServiceFromOrder(int? BranchId = null, DateTime? FromDate = null, DateTime? ToDate = null, int? OutletId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOrderService", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "FromDate", FromDate },
                { "ToDate", ToDate },
                { "OutletId", OutletId }
            });
            var services = ds.Tables[0].ToList<ServiceDTO>();
            
            return services;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var services = await _context.Services.FindAsync(Id);
                    if (services == null)
                    {
                        return false;
                    }
                    services.IsDelete = true;
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