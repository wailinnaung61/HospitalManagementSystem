using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class WarehouseTransferRepository : Repository<WarehouseTransfer>, IWarehouseTransferRepository
    {
        public WarehouseTransferRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<WarehouseTransfer> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWarehouseTransfers");
            var warehouseTransfers = ds.Tables[0].ToList<WarehouseTransfer>();
            return warehouseTransfers;
        }
        public override List<WarehouseTransfer> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWarehouseTransfers", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var warehouseTransfers = ds.Tables[0].ToList<WarehouseTransfer>();
            return warehouseTransfers;
        }
        public List<WarehouseTransfer> GetAll(int? BranchId, int? WarehouseTransferId, int? FromWarehouseId, int? ToWarehouseId, int? ItemId, string Remark = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWarehouseTransfers", new Dictionary<string, object>()
            {
                { "BranchId", BranchId },
                { "WarehouseTransferId", WarehouseTransferId },
                { "FromWarehouseId", FromWarehouseId },
                { "ToWarehouseId", ToWarehouseId },
                { "ItemId", ItemId },
                { "Remark", Remark },
                { "Date", Date },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
            });
            var warehouseTransfers = ds.Tables[0].ToList<WarehouseTransfer>();
            return warehouseTransfers;
        }
        public List<WarehouseTransferItem> GetWarehouseTransferItems(int WarehouseTransferId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWarehouseTransferItems", new Dictionary<string, object>() { { "WarehouseTransferId", WarehouseTransferId } });
            var warehouseTransferItems = ds.Tables[0].ToList<WarehouseTransferItem>();
            return warehouseTransferItems;
        }
        public List<WarehouseItem> GetWarehouseTransferItemsForUpdate(int WarehouseTransferId, int WarehouseId)
        {
            var warehouseTransferItems = GetWarehouseTransferItems(WarehouseTransferId);
            return warehouseTransferItems.Select(x => new WarehouseItem
            {
                WarehouseId = WarehouseId,
                ItemId = x.ItemId,
                BatchId = x.BatchId,
                Qty = x.QtyInSmallestUnit
            }).GroupBy(x => new { x.WarehouseId, x.ItemId, x.BatchId }, (key, g) => new WarehouseItem
            {
                WarehouseId = WarehouseId,
                ItemId = g.First().ItemId,
                BatchId = g.First().BatchId,
                Qty = g.Sum(i => i.Qty)
            }).ToList();
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
        public override WarehouseTransfer Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWarehouseTransfers", new Dictionary<string, object>() { { "WarehouseTransferId", Id } });
            var warehouseTransfers = ds.Tables[0].ToList<WarehouseTransfer>();
            var warehouseTransfer = warehouseTransfers.Count > 0 ? warehouseTransfers[0] : null;
            if (warehouseTransfer != null)
            {
                var warehouseTransferItems = GetWarehouseTransferItems(warehouseTransfer.Id);
                warehouseTransferItems.ForEach(x =>
                {
                    x.Item = GetItem(x.ItemId);
                    x.Unit = GetUnit(x.UnitId);
                    x.Batch = GetBatch(x.BatchId);
                    x.ItemName = x.Item.Name;
                    x.BatchName = x.Batch.Name;
                });
                warehouseTransfer.WarehouseTransferItems = warehouseTransferItems;
            }
            return warehouseTransfer;
        }
        public override async Task<WarehouseTransfer> UpdateAsync(WarehouseTransfer warehouseTransfer)
        {
            if (warehouseTransfer != null)
            {
                warehouseTransfer.WarehouseTransferItems.ToList().ForEach(x => x.WarehouseTransferId = warehouseTransfer.Id);
                var existingWarehouseTransfer = _context.WarehouseTransfers
                    .Where(p => p.Id == warehouseTransfer.Id)
                    .Include(p => p.WarehouseTransferItems)
                    .SingleOrDefault();

                if (existingWarehouseTransfer != null)
                {
                    // Update parent
                    _context.Entry(existingWarehouseTransfer).CurrentValues.SetValues(warehouseTransfer);

                    // Delete children
                    _context.WarehouseTransferItems.RemoveRange(existingWarehouseTransfer.WarehouseTransferItems);

                    // Insert children
                    existingWarehouseTransfer.WarehouseTransferItems = warehouseTransfer.WarehouseTransferItems;

                    await _context.SaveChangesAsync();
                    return warehouseTransfer;
                }
            }
            return null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            try
            {
                var warehouseTransfer = await _context.WarehouseTransfers.FindAsync(Id);
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