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
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static MSIS_HMS.Infrastructure.Enums.DbEnum;
using MSIS_HMS.Core.Entities.DTOs;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override List<Warehouse> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWarehouses");
            var warehouses = ds.Tables[0].ToList<Warehouse>();
            return warehouses;
        }
        public override List<Warehouse> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWarehouses", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var warehouses = ds.Tables[0].ToList<Warehouse>();
            return warehouses;
        }
        public override Warehouse Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWarehouses", new Dictionary<string, object>() { { "WarehouseId", Id } });
            var warehouses = ds.Tables[0].ToList<Warehouse>();
            return warehouses.Count > 0 ? warehouses[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var warehouse = await _context.Warehouses.FindAsync(Id);
                    if (warehouse == null)
                    {
                        return false;
                    }
                    warehouse.IsDelete = true;
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
        public List<WarehouseItemDTO> GetWarehouseItemDTOs(int? BranchId, int? WarehouseId, int? ItemId, int? BatchId, DateTime? ExpiryDate, DateTime? StartExpiryDate, DateTime? EndExpiryDate)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWarehouseItems", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "WarehouseId", WarehouseId },
                { "ItemId", ItemId },
                { "BatchId", BatchId },
                { "ExpiryDate", ExpiryDate },
                { "StartExpiryDate", StartExpiryDate },
                { "EndExpiryDate", EndExpiryDate },
            });
            var warehouseItems = ds.Tables[0].ToList<WarehouseItemDTO>();
            
            return warehouseItems;
        }
        public List<WarehouseItemDTO> GetWarehouseItemReport(int? BranchId, int? WarehouseId, int? ItemId, int? BatchId, DateTime? ExpiryDate, DateTime? StartExpiryDate, DateTime? EndExpiryDate)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWarehouseItems", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "WarehouseId", WarehouseId },
                { "ItemId", ItemId },
                { "BatchId", BatchId },
                { "ExpiryDate", ExpiryDate },
                { "StartExpiryDate", StartExpiryDate },
                { "EndExpiryDate", EndExpiryDate },
            });
            var warehouseItems = ds.Tables[0].ToList<WarehouseItemDTO>();
            foreach(var whi in warehouseItems)
            {
                var itemInfo = _context.Items.Where(x => x.Id == whi.ItemId).FirstOrDefault();
                var day =(Convert.ToDateTime(whi.ExpiryDate)- DateTime.Now).Days;
                if(day<=itemInfo.ExpirationRemindDay)
                {
                    whi.NearExpiry = true;
                }
                else
                {
                    whi.NearExpiry = false;
                }
                List<PackingUnit> packingUnits = _context.PackingUnits.Where(x => x.ItemId == whi.ItemId).ToList();
                IDictionary<int, int> result = InventoryExtensions.GetUnitFromSamllestQty(packingUnits, whi.Qty);
                foreach (var r in result.OrderBy(x => x.Key))
                {
                    if(whi.Qty<=0)
                    {
                        whi.QtyString = "0";
                    }
                    else
                    {
                        if (r.Value != 0)
                        {
                            var unitShortName = _context.Units.Where(x => x.Id == r.Key).FirstOrDefault();
                            whi.QtyString += r.Value.ToString() + unitShortName.ShortForm + " ";
                        }
                    }                     
                }
            }
            return warehouseItems;
        }
        public List<Item> GetItemsFromWarehouse(int BranchId, int WarehouseId)
        {
            var warehouseItems = GetWarehouseItemDTOs(BranchId, WarehouseId, null, null, null, null, null);
            var items = warehouseItems.Where(x => x.Qty > 0).GroupBy(x => x.ItemId, (key, g) => GetItem(key)).ToList();
            
            return items;
        }
        public List<Batch> GetBatchesOfWarehouseItem(int BranchId, int WarehouseId, int ItemId)
        {
            var warehouseItems = GetWarehouseItemDTOs(BranchId, WarehouseId, ItemId, null, null, null, null);
            var batches = warehouseItems.Select(x => GetBatch(x.BatchId)).ToList();
            return batches;
        }


        private Item GetItem(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItems", new Dictionary<string, object>() { { "ItemId", Id } });
            var items = ds.Tables[0].ToList<Item>();
            items.ForEach(x => x.PackingUnits = GetPackingUnits(x.Id));
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
        private List<PackingUnit> GetPackingUnits(int itemId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPackingUnits", new Dictionary<string, object>() { { "ItemId", itemId } });
            var packingUnits = ds.Tables[0].ToList<PackingUnit>();
            packingUnits.ForEach(x =>
            {
                x.UnitName = GetUnit(x.UnitId).Name;
            });
            return packingUnits.OrderBy(x => x.UnitLevel).ToList();
        }
        public async Task UpdateStockAsync(List<WarehouseItem> warehouseItems, int? action = null)
        {
            var existingWarehouseItems = _context.WarehouseItems.AsEnumerable().Where(x => warehouseItems.AsEnumerable().Any(wi => wi.WarehouseId == x.WarehouseId && wi.ItemId == x.ItemId && wi.BatchId == x.BatchId)).ToList();
            foreach (var warehouseItem in warehouseItems)
            {
                var existingWarehouseItem = existingWarehouseItems
                    .Where(x => x.WarehouseId == warehouseItem.WarehouseId && warehouseItem.ItemId == x.ItemId && x.BatchId == warehouseItem.BatchId && x.WarehouseId != default(int) && x.ItemId != default(int) && x.BatchId != default(int))
                    .SingleOrDefault();

                if (existingWarehouseItem != null)
                {
                    // Update child
                    switch (action)
                    {
                        //case DbActionEnum.Update: existingWarehouseItem.Qty += warehouseItem.Qty; break;
                        case (int)DbActionEnum.Delete: existingWarehouseItem.Qty -= warehouseItem.Qty; break;
                        default: existingWarehouseItem.Qty += warehouseItem.Qty; break;
                    }
                }
                else
                {
                    // Insert child
                    _context.WarehouseItems.Add(warehouseItem);
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task ReplaceStockAsync(List<WarehouseItem> warehouseItems)
        {
            var existingWarehouseItems = _context.WarehouseItems.AsEnumerable().Where(x => warehouseItems.AsEnumerable().Any(wi => wi.WarehouseId == x.WarehouseId && wi.ItemId == x.ItemId && wi.BatchId == x.BatchId)).ToList();
            foreach (var warehouseItem in warehouseItems)
            {
                var existingWarehouseItem = existingWarehouseItems
                    .Where(x => x.WarehouseId == warehouseItem.WarehouseId && warehouseItem.ItemId == x.ItemId && x.BatchId == warehouseItem.BatchId && x.WarehouseId != default(int) && x.ItemId != default(int) && x.BatchId != default(int))
                    .SingleOrDefault();
                if (existingWarehouseItem != null)
                {
                    // Update child
                   existingWarehouseItem.Qty = warehouseItem.Qty; 
                    
                }
                else
                {
                    // Insert child
                    _context.WarehouseItems.Add(warehouseItem);
                }
                await _context.SaveChangesAsync();
            }
        }

        public List<WarehouseItemLocationDTO> GetWarehouseItemLocationsDTO(int? BranchId, int? WarehouseId, int? ItemId, int? LocationId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItemWithExpirationRemindDate", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "WarehouseId", WarehouseId },
                { "ItemId", ItemId },
                {"LocationId",LocationId }
                
            });
            var warehouseItems = ds.Tables[0].ToList<WarehouseItemLocationDTO>();
            return warehouseItems;
        }
    }
}