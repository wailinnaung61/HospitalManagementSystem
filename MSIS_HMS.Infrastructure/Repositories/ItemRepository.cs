using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }

        public override List<Item> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItems");
            var items = ds.Tables[0].ToList<Item>();
            return items;
        }

        public override List<Item> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItems", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var items = ds.Tables[0].ToList<Item>();
            return items;
        }
        public List<Item> GetAll(int? BranchId = null, int? ItemId = null, int? ItemTypeId = null, string Name = null, string Code = null, string Barcode = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItems", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "ItemId", ItemId },
                { "ItemTypeId", ItemTypeId },
                { "Name", Name },
                { "Code", Code },
                { "Barcode", Barcode }
            });
            var items = ds.Tables[0].ToList<Item>();
            return items;
        }
        public List<Item> GetAllWithPackingUnits()
        {
            var items = GetAll();
            items.ForEach(x => x.PackingUnits = GetPackingUnits(x.Id));
            return items;
        }

        public List<Item> GetAllWithPackingUnits(int? BranchId)
        {
            var items = GetAll(BranchId);
            items.ForEach(x => x.PackingUnits = GetPackingUnits(x.Id));
            return items;
        }

        public List<Item> GetAllWithPackingUnits(int? BranchId = null, int? ItemId = null, int? ItemTypeId = null, string Name = null, string Code = null, string Barcode = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItems", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "ItemId", ItemId },
                { "ItemTypeId", ItemTypeId },
                { "Name", Name },
                { "Code", Code },
                { "Barcode", Barcode }
            });
            var items = ds.Tables[0].ToList<Item>();
            items.ForEach(x => x.PackingUnits = GetPackingUnits(x.Id));
            return items;
        }

        public override Item Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItems", new Dictionary<string, object>() { { "ItemId", Id } });
            var items = ds.Tables[0].ToList<Item>();
            return items.Count > 0 ? items[0] : null;
        }

        public Item GetWithPackingUnit(int Id)
        {
            var item = Get(Id);
            if (item != null)
            {
                item.PackingUnits = GetPackingUnits(item.Id);
            }
            return item;
        }

        public List<PackingUnit> GetPackingUnits(int itemId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPackingUnits", new Dictionary<string, object>() { { "ItemId", itemId } });
            var packingUnits = ds.Tables[0].ToList<PackingUnit>();
            packingUnits.ForEach(x =>
            {
                //var unit = GetUnit(x.UnitId);
                //x.Unit = unit;
                x.UnitName = GetUnit(x.UnitId).Name;
            });
            return packingUnits.OrderBy(x => x.UnitLevel).ToList();
        }

        public Unit GetUnit(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetUnits", new Dictionary<string, object>() { { "UnitId", Id } });
            var units = ds.Tables[0].ToList<Unit>();
            return units.Count > 0 ? units[0] : null;
        }

        public List<Item> GetExpirationRemindDay(int? BranchId = null, int? ItemId = null, int? ItemTypeId = null, string Name = null,
            string Code = null, string Barcode = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetItemsForExpirationRemind", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "ItemId", ItemId },
                { "ItemTypeId", ItemTypeId },
                { "Name", Name },
                { "Code", Code },
                { "Barcode", Barcode },
            });
            var items = ds.Tables[0].ToList<Item>();
            return items;
        }
        public List<SaleItemDTO> GetSaleItem(int? BranchId=null,DateTime? FromDate=null,DateTime? ToDate=null,int? OutletId=null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetSaleItem", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "FromDate", FromDate },
                { "ToDate", ToDate },
                { "OutletId", OutletId }                
            });
            var items = ds.Tables[0].ToList<SaleItemDTO>();
            foreach (var i in items)
            {
                List<PackingUnit> packingUnits = _context.PackingUnits.Where(x => x.ItemId == i.Id).ToList();
                IDictionary<int,int> result = InventoryExtensions.GetUnitFromSamllestQty(packingUnits, i.Qty);
                foreach(var r in result.OrderBy(x => x.Key))
                {
                    var unitShortName = _context.Units.Where(x => x.Id == r.Key).FirstOrDefault();
                    i.QtyString += r.Value.ToString() + unitShortName.ShortForm + " ";
                }
                
            }
            return items;
        }
        public override async Task<Item> UpdateAsync(Item item)
        {
            if (item != null)
            {
                try
                {
                    var existingItem = _context.Items
                        .Where(p => p.Id == item.Id)
                        .Include(p => p.PackingUnits)
                        .SingleOrDefault();

                    if (existingItem != null)
                    {
                        // Update parent
                        _context.Entry(existingItem).CurrentValues.SetValues(item);

                        // Delete children
                        foreach (var existingPackingUnit in existingItem.PackingUnits.ToList())
                        {
                            if (!item.PackingUnits.Any(c => c.UnitId == existingPackingUnit.UnitId))
                                _context.PackingUnits.Remove(existingPackingUnit);
                        }

                        // Update and Insert children
                        foreach (var packingUnit in item.PackingUnits)
                        {
                            var existingPackingUnit = existingItem.PackingUnits
                                .Where(c => c.UnitId == packingUnit.UnitId && c.UnitId != default(int))
                                .SingleOrDefault();

                            if (existingPackingUnit != null)
                                // Update child
                                _context.Entry(existingPackingUnit).CurrentValues.SetValues(packingUnit);
                            else
                            {
                                // Insert child
                                existingItem.PackingUnits.Add(packingUnit);
                            }
                        }

                        await _context.SaveChangesAsync();
                        return item;
                    }
                }
                catch (DbException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return null;
        }

        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var item = await _context.Items.FindAsync(Id);
                    if (item == null)
                    {
                        return false;
                    }
                    item.IsDelete = true;
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