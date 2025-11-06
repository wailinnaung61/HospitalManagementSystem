using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
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
using static MSIS_HMS.Infrastructure.Enums.DbEnum;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class OutletRepository : Repository<Outlet>, IOutletRepository
    {
        public OutletRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public  List<Outlet> GetAll(int? BranchId=null,string OutletName = null, string OutletCode = null, int? WarehouseId = null,int? OutletId=null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOutlets",new Dictionary<string, object> {
                {"BranchId",BranchId },
                {"OutletName",OutletName },
                {"OutletCode",OutletCode },
                {"WarehouseId",WarehouseId },
                {"OutletId",OutletId }
            });
            var outlets = ds.Tables[0].ToList<Outlet>();
            return outlets;
        }
        public override Outlet Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOutlets", new Dictionary<string, object>() { { "@OutletId", Id } });
            var outlets = ds.Tables[0].ToList<Outlet>();
            return outlets.Count > 0 ? outlets[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var outlet = await _context.Outlets.FindAsync(Id);
                    if (outlet == null)
                    {
                        return false;
                    }
                    outlet.IsDelete = true;
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

        //public async Task UpdateStockAsync(List<OutletItem> outletItems,List<WarehouseItem> warehouseItems, int? action = null)
        //{
        //    var existingOutletItems = _context.OutletItems.AsEnumerable().Where(x => outletItems.AsEnumerable().Any(ou => ou.OutletId == x.OutletId && ou.ItemId == x.ItemId)).ToList();
        //    foreach (var outletItem in outletItems)
        //    {
        //        var existingOutletItem = existingOutletItems
        //            .Where(x => x.OutletId == outletItem.OutletId && outletItem.ItemId == x.ItemId && x.OutletId != default(int) && x.ItemId != default(int))
        //            .SingleOrDefault();

        //        if (existingOutletItem != null)
        //        {
        //            // Update child
        //            switch (action)
        //            {
        //                //case DbActionEnum.Update: existingWarehouseItem.Qty += warehouseItem.Qty; break;
        //                case (int)DbActionEnum.Delete: existingOutletItem.Qty -= outletItem.Qty; break;
        //                default: existingOutletItem.Qty += outletItem.Qty; break;
        //            }
        //        }
        //        else
        //        {
        //            // Insert child
        //            _context.OutletItems.Add(outletItem);
        //        }
        //    }

        //    var existingWarehouseItems = _context.WarehouseItems.AsEnumerable().Where(x => warehouseItems.AsEnumerable().Any(wi => wi.WarehouseId == x.WarehouseId && wi.ItemId == x.ItemId)).ToList();
        //    foreach (var warehouseItem in warehouseItems)
        //    {
        //        var existingWarehouseItem = existingWarehouseItems
        //            .Where(x => x.WarehouseId == warehouseItem.WarehouseId && warehouseItem.ItemId == x.ItemId && x.WarehouseId != default(int) && x.ItemId != default(int) && warehouseItem.BatchId==x.BatchId)
        //            .SingleOrDefault();

        //        if (existingWarehouseItem != null)
        //        {
        //            // Update child
        //            switch (action)
        //            {
        //                //case DbActionEnum.Update: existingWarehouseItem.Qty += warehouseItem.Qty; break;
        //                case (int)DbActionEnum.Delete: existingWarehouseItem.Qty -= warehouseItem.Qty; break;
        //                default: existingWarehouseItem.Qty += warehouseItem.Qty; break;
        //            }
        //        }
        //        else
        //        {
        //            // Insert child
        //            _context.WarehouseItems.Add(warehouseItem);
        //        }
        //    }
        //    await _context.SaveChangesAsync();
        //}
        public async Task UpdateStockAsync(List<OutletItem> outletItems, int? action = null)
        {
            var existingOutletItems = _context.OutletItems.AsEnumerable().Where(x => outletItems.AsEnumerable().Any(ou => ou.OutletId == x.OutletId && ou.ItemId == x.ItemId)).ToList();
            foreach (var outletItem in outletItems)
            {
                var existingOutletItem = existingOutletItems
                    .Where(x => x.OutletId == outletItem.OutletId && outletItem.ItemId == x.ItemId && x.OutletId != default(int) && x.ItemId != default(int))
                    .SingleOrDefault();

                if (existingOutletItem != null)
                {
                    // Update child
                    switch (action)
                    {
                        //case DbActionEnum.Update: existingWarehouseItem.Qty += warehouseItem.Qty; break;
                        case (int)DbActionEnum.Delete: existingOutletItem.Qty -= outletItem.Qty; break;
                        default: existingOutletItem.Qty += outletItem.Qty; break;
                    }
                }
                else
                {
                    // Insert child
                    _context.OutletItems.Add(outletItem);
                }
            }

            await _context.SaveChangesAsync();
        }
        public List<Item> GetItemsFromOutlet(int BranchId, int WarehouseId)
        {
            var outletItems = GetOutletItemDTOs(BranchId, WarehouseId, null);
            var items = outletItems.Where(x => x.Qty > 0).GroupBy(x => x.ItemId, (key, g) => GetItem(key)).ToList();
            return items;
        }

        public List<OutletItemDTO> GetOutletItemDTOs(int? BranchId, int? OutletId, int? ItemId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOutletItems", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "OutletId", OutletId },
                { "ItemId", ItemId }

            });
            var outletItems = ds.Tables[0].ToList<OutletItemDTO>();
            return outletItems;
        }
        public List<OutletStockItemDTO> GetOutetStocks(int? BranchId,int? WarehouseId,int? OutletId,int? ItemId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOutletStock", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "WarehouseId", WarehouseId },
                { "OutletId", OutletId },
                { "ItemId", ItemId }

            });
            var outletItems = ds.Tables[0].ToList<OutletStockItemDTO>();
            foreach (var i in outletItems)
            {
                List<PackingUnit> packingUnits = _context.PackingUnits.Where(x => x.ItemId == i.ItemId).ToList();
                IDictionary<int, int> result = InventoryExtensions.GetUnitFromSamllestQty(packingUnits, i.Qty);
                foreach (var r in result.OrderBy(x => x.Key))
                {
                    if (i.Qty <= 0)
                    {
                        i.QtyString = "0";
                    }
                    else
                    {
                        if (r.Value != 0)
                        {
                            var unitShortName = _context.Units.Where(x => x.Id == r.Key).FirstOrDefault();
                            i.QtyString += r.Value.ToString() + unitShortName.ShortForm + " ";
                        }
                    }
                }
            }
            return outletItems;
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
    }
}
