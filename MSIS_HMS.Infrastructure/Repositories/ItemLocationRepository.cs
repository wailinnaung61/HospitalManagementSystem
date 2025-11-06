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
    public class ItemLocationRepository : Repository<ItemLocation>,IItemLocationRepository
    {
        public ItemLocationRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public  List<ItemLocation> GetAll(int? BranchId=null,int? ItemId=null,int? LocationId=null,int? ItemLocationId=null,int? BatchId=null,int? WarehouseId=null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItemLocations",new Dictionary<string, object> {
                {"ItemId",ItemId },
                {"LocationId",LocationId },
                {"BatchId",BatchId },
                {"WarehouseId",WarehouseId }
            });
            var itemLocations = ds.Tables[0].ToList<ItemLocation>();
            return itemLocations;        
        }
        public override ItemLocation Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItemLocations", new Dictionary<string, object>() { { "ItemLocationId", Id } });
            var itemLocations = ds.Tables[0].ToList<ItemLocation>();
            return itemLocations.Count > 0 ? itemLocations[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var itemLocation = await _context.ItemLocations.FindAsync(Id);
                    if (itemLocation == null)
                    {
                        return false;
                    }
                    itemLocation.IsDelete = true;
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