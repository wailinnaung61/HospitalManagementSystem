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
    public class OutletTransferRepository : Repository<OutletTransfer>, IOutletTransferRepository
    {
        public OutletTransferRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<OutletTransfer> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOutletTransfers");
            var outletTransfers = ds.Tables[0].ToList<OutletTransfer>();
            return outletTransfers;
        }
        public List<OutletTransfer> GetAll(int? BranchId = null, int? warehouseId = null, int? outletId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOutletTransfers", new Dictionary<string, object>()
            { { "BranchId", BranchId },
              { "FromWarehouseId", warehouseId },
              { "ToOutletId", outletId },
              { "StartDate", fromDate },
              { "EndDate", toDate } });
            var outletTransfers = ds.Tables[0].ToList<OutletTransfer>();
            return outletTransfers;
        }
        public override OutletTransfer Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOutletTransfers", new Dictionary<string, object>() { { "OutletTransferId", Id } });
            var outletTransfers = ds.Tables[0].ToList<OutletTransfer>();
            var outletTransfer = outletTransfers.Count > 0 ? outletTransfers[0] : null;
            if (outletTransfer != null)
            {
                var outletTransferItems = GetWarehouseTransferItems(outletTransfer.Id);
                outletTransferItems.ForEach(x =>
                {
                    x.Item = GetItem(x.ItemId);
                    x.Unit = GetUnit(x.UnitId);
                    x.Batch = GetBatch(x.BatchId);
                    x.ItemName = x.Item.Name;
                    x.BatchName = x.Batch.Name;
                });
                outletTransfer.OutletTransferItems = outletTransferItems;
            }
            return outletTransfer;
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
        public List<OutletTransferItem> GetWarehouseTransferItems(int outletTransferId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOutletTransferItems", new Dictionary<string, object>() { { "OutletTransferId", outletTransferId } });
            var outletTransferItems = ds.Tables[0].ToList<OutletTransferItem>();
            return outletTransferItems;
        }
        public List<WarehouseItem> GetWarehouseTransferItemsForUpdate(int OutletTransferId, int warehouseId)
        {
            var warehouseTransferItems = GetWarehouseTransferItems(OutletTransferId);
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
        public List<OutletItem> GetOutletTransferItemsForUpdate(int OutletTransferId, int outletId)
        {
            var warehouseTransferItems = GetWarehouseTransferItems(OutletTransferId);
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

        public override async Task<OutletTransfer> UpdateAsync(OutletTransfer outletTransfer)
        {
            if (outletTransfer != null)
            {
                outletTransfer.OutletTransferItems.ToList().ForEach(x => x.OutletTransferId = outletTransfer.Id);
                var existingOutletTransfer = _context.OutletTransfers
                    .Where(p => p.Id == outletTransfer.Id)
                    .Include(p => p.OutletTransferItems)
                    .SingleOrDefault();

                if (existingOutletTransfer != null)
                {
                    // Update parent
                    _context.Entry(existingOutletTransfer).CurrentValues.SetValues(outletTransfer);

                    // Delete children
                    _context.OutletTransferItems.RemoveRange(existingOutletTransfer.OutletTransferItems);

                    // Insert children
                    existingOutletTransfer.OutletTransferItems = outletTransfer.OutletTransferItems;

                    await _context.SaveChangesAsync();
                    return outletTransfer;
                }
            }
            return null;
        }

        public override async Task<bool> DeleteAsync(int Id)
        {
            try
            {
                var warehouseTransfer = await _context.OutletTransfers.FindAsync(Id);
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
