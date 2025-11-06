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
    public class ItemTypeRepository : Repository<ItemType>,IItemTypeRepository
    {
        public ItemTypeRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
            
        }
        public override List<ItemType> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItemTypes");
            var itemtypes = ds.Tables[0].ToList<ItemType>();
            return itemtypes;
        }
        public override List<ItemType> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItemTypes", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var itemtypes = ds.Tables[0].ToList<ItemType>();
            return itemtypes;
        }
        public override ItemType Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItemTypes", new Dictionary<string, object>() { { "ItemTypeId", Id } });
            var itemTypes = ds.Tables[0].ToList<ItemType>();
            return itemTypes.Count > 0 ? itemTypes[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var itemType = await _context.ItemTypes.FindAsync(Id);
                    if (itemType == null)
                    {
                        return false;
                    }
                    itemType.IsDelete = true;
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