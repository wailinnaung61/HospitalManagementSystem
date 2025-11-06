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
    public class PurchaseRepository : Repository<Purchase>, IPurchaseRepository
    {
        public PurchaseRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Purchase> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPurchases");
            var purchases = ds.Tables[0].ToList<Purchase>();
            return purchases;
        }
        public override List<Purchase> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPurchases", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var purchases = ds.Tables[0].ToList<Purchase>();
            return purchases;
        }
        public List<Purchase> GetAll(int? BranchId, int? PurchaseId, int? WarehouseId, int? ItemId, string VoucherNo = null, string Supplier = null, DateTime? PurchaseDate = null, DateTime? StartPurchaseDate = null, DateTime? EndPurchaseDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPurchases", new Dictionary<string, object>()
            {
                { "BranchId", BranchId },
                { "PurchaseId", PurchaseId },
                { "WarehouseId", WarehouseId },
                { "ItemId", ItemId },
                { "VoucherNo", VoucherNo },
                { "Supplier", Supplier },
                { "PurchaseDate", PurchaseDate },
                { "StartPurchaseDate", StartPurchaseDate },
                { "EndPurchaseDate", EndPurchaseDate },
            });
            var purchases = ds.Tables[0].ToList<Purchase>();
            return purchases;
        }
        public List<PurchaseItem> GetPurchaseItems(int PurchaseId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPurchaseItems", new Dictionary<string, object>() { { "PurchaseId", PurchaseId } });
            var purchaseItems = ds.Tables[0].ToList<PurchaseItem>();
            return purchaseItems;
        }
        public List<WarehouseItem> GetPurchaseItemsForUpdate(int PurchaseId)
        {
            var purchase = Get(PurchaseId);
            return GetPurchaseItems(PurchaseId).Select(x => new WarehouseItem
            {
                WarehouseId = purchase.WarehouseId,
                ItemId = x.ItemId,
                BatchId = x.BatchId,
                Qty = x.QtyInSmallestUnit
            }).GroupBy(x => new { x.WarehouseId, x.ItemId, x.BatchId }, (key, g) => new WarehouseItem
            {
                WarehouseId = purchase.WarehouseId,
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
        public override Purchase Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPurchases", new Dictionary<string, object>() { { "PurchaseId", Id } });
            var purchases = ds.Tables[0].ToList<Purchase>();
            var purchase = purchases.Count > 0 ? purchases[0] : null;
            if (purchase != null)
            {
                var purchaseItems = GetPurchaseItems(purchase.Id);
                purchaseItems.ForEach(x =>
                {
                    x.Item = GetItem(x.ItemId);
                    x.Unit = GetUnit(x.UnitId);
                    x.Batch = GetBatch(x.BatchId);
                    x.ItemName = x.Item.Name;
                    x.BatchName = x.Batch.Name;
                });
                purchase.PurchaseItems = purchaseItems;
            }
            return purchase;
        }
        public override async Task<Purchase> UpdateAsync(Purchase purchase)
        {
            if (purchase != null)
            {
                purchase.PurchaseItems.ToList().ForEach(x => x.PurchaseId = purchase.Id);
                var existingPurchase = _context.Purchases
                    .Where(p => p.Id == purchase.Id)
                    .Include(p => p.PurchaseItems)
                    .SingleOrDefault();

                if (existingPurchase != null)
                {
                    // Update parent
                    _context.Entry(existingPurchase).CurrentValues.SetValues(purchase);

                    // Delete children
                    _context.PurchaseItems.RemoveRange(existingPurchase.PurchaseItems);

                    // Insert children
                    existingPurchase.PurchaseItems = purchase.PurchaseItems;

                    await _context.SaveChangesAsync();
                    return purchase;
                }
            }
            return null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            try
            {
                var purchase = await _context.Purchases.FindAsync(Id);
                if (purchase == null)
                {
                    return false;
                }
                purchase.IsDelete = true;
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