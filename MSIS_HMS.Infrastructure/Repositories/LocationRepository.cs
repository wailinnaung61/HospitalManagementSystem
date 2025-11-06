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
using System.Text;
using System.Threading.Tasks;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class LocationRepository : Repository<Location>, ILocationRepository
    {
        public LocationRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public List<Location> GetAll(int? BranchId = null, string LocationName = null, string LocationCode = null, int? WarehouseId = null, int? LocationId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLocations", new Dictionary<string, object>() {
                {"BranchId",BranchId },
                { "LocationName", LocationName },
                { "LocationCode", LocationCode },
                { "WarehouseId", WarehouseId },
                { "Locationid" , LocationId }

            });
            var locatons = ds.Tables[0].ToList<Location>();
            return locatons;
        }
        public override Location Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLocations", new Dictionary<string, object>() { { "LocationId", Id } });
            var locations = ds.Tables[0].ToList<Location>();
            return locations.Count > 0 ? locations[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var location = await _context.Locations.FindAsync(Id);
                    if (location == null)
                    {
                        return false;
                    }
                    location.IsDelete = true;
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
