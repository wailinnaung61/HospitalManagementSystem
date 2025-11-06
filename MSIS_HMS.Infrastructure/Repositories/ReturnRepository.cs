using Microsoft.EntityFrameworkCore;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class ReturnRepository:Repository<Return>, IReturnRepository
    {
        public ReturnRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Return> GetAll(int? BranchId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetReturns", new Dictionary<string, object>()
            { { "BranchId", BranchId } });
            var returnModel = ds.Tables[0].ToList<Return>();
            return returnModel;
        }
        public List<Return> GetAll(int? BranchId = null, int? warehouseId = null, int? outletId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetReturns", new Dictionary<string, object>()
            { { "BranchId", BranchId },
              { "ToWarehouseId", warehouseId },
              { "FromOutletId", outletId },
              { "StartDate", fromDate },
              { "EndDate", toDate } });
            var returnModel = ds.Tables[0].ToList<Return>();
            return returnModel;
        }
        public override Return Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetReturns", new Dictionary<string, object>() { { "ReturnId", Id } });
            var returnModels = ds.Tables[0].ToList<Return>();
            var returnModel = returnModels.Count > 0 ? returnModels[0] : null;
            if (returnModel != null)
            {
                var outletTransferItems = GetWarehouseTransferItems(returnModel.Id);
                outletTransferItems.ForEach(x =>
                {
                    x.Item = GetItem(x.ItemId);
                    x.Unit = GetUnit(x.UnitId);
                    x.Batch = GetBatch(x.BatchId);
                    x.ItemName = x.Item.Name;
                    x.BatchName = x.Batch.Name;
                });
                returnModel.ReturnItems = outletTransferItems;
            }
            return returnModel;
        }
        private Item GetItem(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItems", new Dictionary<string, object>() { { "ItemId", Id } });
            var items = ds.Tables[0].ToList<Item>();
            return items.Count > 0 ? items[0] : null;
        }
        private Unit GetUnit(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetUnits", new Dictionary<string, object>() { { "UnitId", Id } });
            var units = ds.Tables[0].ToList<Unit>();
            return units.Count > 0 ? units[0] : null;
        }
        private Batch GetBatch(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBatches", new Dictionary<string, object>() { { "BatchId", Id } });
            var batches = ds.Tables[0].ToList<Batch>();
            return batches.Count > 0 ? batches[0] : null;
        }
        public List<ReturnItem> GetWarehouseTransferItems(int ReturnId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetReturnItems", new Dictionary<string, object>() { { "ReturnId", ReturnId } });
            var returnItems = ds.Tables[0].ToList<ReturnItem>();
            return returnItems;
        }
        public List<WarehouseItem> GetWarehouseTransferItemsForUpdate(int ReturnId, int warehouseId)
        {
            var warehouseTransferItems = GetWarehouseTransferItems(ReturnId);
            return warehouseTransferItems.Select(x => new WarehouseItem
            {
                WarehouseId = warehouseId,
                ItemId = x.ItemId,
                BatchId = x.BatchId,
                Qty = x.QtyInSmallestUnit
            }).GroupBy(x => new { x.WarehouseId, x.ItemId }, (key, g) => new WarehouseItem
            {
                WarehouseId = warehouseId,
                ItemId = g.First().ItemId,
                BatchId = g.First().BatchId,
                Qty = g.Sum(i => i.Qty)
            }).ToList();
        }
        public List<OutletItem> GetOutletTransferItemsForUpdate(int ReturnId, int outletId)
        {
            var warehouseTransferItems = GetWarehouseTransferItems(ReturnId);
            return warehouseTransferItems.Select(x => new OutletItem
            {
                OutletId = outletId,
                ItemId = x.ItemId,
                Qty = x.QtyInSmallestUnit
            }).GroupBy(x => new { x.OutletId, x.ItemId }, (key, g) => new OutletItem
            {
                OutletId = outletId,
                ItemId = g.First().ItemId,
                Qty = g.Sum(i => i.Qty)
            }).ToList();
        }

        public override async Task<Return> UpdateAsync(Return returnModel)
        {
            if (returnModel != null)
            {
                returnModel.ReturnItems.ToList().ForEach(x => x.ReturnId = returnModel.Id);
                var existingOutletTransfer = _context.Returns
                    .Where(p => p.Id == returnModel.Id)
                    .Include(p => p.ReturnItems)
                    .SingleOrDefault();

                if (existingOutletTransfer != null)
                {
                    // Update parent
                    _context.Entry(existingOutletTransfer).CurrentValues.SetValues(returnModel);

                    // Delete children
                    _context.ReturnItems.RemoveRange(existingOutletTransfer.ReturnItems);

                    // Insert children
                    existingOutletTransfer.ReturnItems = returnModel.ReturnItems;

                    await _context.SaveChangesAsync();
                    return returnModel;
                }
            }
            return null;
        }

        public override async Task<bool> DeleteAsync(int Id)
        {
            try
            {
                var warehouseTransfer = await _context.Returns.FindAsync(Id);
                if (warehouseTransfer == null)
                {
                    return false;
                }
                warehouseTransfer.IsDelete = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
    }
}
