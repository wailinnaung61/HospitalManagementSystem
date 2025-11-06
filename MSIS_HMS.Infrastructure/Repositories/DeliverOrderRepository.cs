using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class DeliverOrderRepository:Repository<DeliverOrder>,IDeliverOrderRepository
    {
        public DeliverOrderRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        
        public override List<DeliverOrder> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDeliverOrders");
            var deliverOrders = ds.Tables[0].ToList<DeliverOrder>();
            return deliverOrders;
        }
        public override List<DeliverOrder> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDeliverOrders", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var deliverOrders = ds.Tables[0].ToList<DeliverOrder>();
            return deliverOrders;
        }
        public List<DeliverOrder> GetAll(int? BranchId, int? DeliverOrderId, int? WarehouseId, int? ItemId, string VoucherNo = null, string Customer = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDeliverOrders", new Dictionary<string, object>()
            {
                { "BranchId", BranchId },
                { "DeliverOrderId", DeliverOrderId },
                { "WarehouseId", WarehouseId },
                { "ItemId", ItemId },
                { "VoucherNo", VoucherNo },
                { "Customer", Customer },
                { "Date", Date },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
            });
            var deliverOrders = ds.Tables[0].ToList<DeliverOrder>();
            return deliverOrders;
        }
        
        public List<DeliverOrderItem> GetDeliverOrderItems(int DeliverOrderId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDeliverOrderItems", new Dictionary<string, object>() { { "DeliverOrderId", DeliverOrderId } });
            var deliverOrderItems = ds.Tables[0].ToList<DeliverOrderItem>();
            return deliverOrderItems;
        }
        
        public List<WarehouseItem> GetDeliverOrderItemsForUpdate(int DeliverOrderId)
        {
            var deliverOrder = Get(DeliverOrderId);
            
            return GetDeliverOrderItems(DeliverOrderId).Select(x => new WarehouseItem
            {
                WarehouseId = deliverOrder.WarehouseId,
                ItemId = x.ItemId,
                BatchId = x.BatchId,
                Qty = x.QtyInSmallestUnit
            }).GroupBy(x => new { x.WarehouseId, x.ItemId, x.BatchId }, (key, g) => new WarehouseItem
            {
                WarehouseId = deliverOrder.WarehouseId,
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
        
        public override DeliverOrder Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDeliverOrders", new Dictionary<string, object>() { { "DeliverOrderId", Id } });
            var deliverOrders = ds.Tables[0].ToList<DeliverOrder>();
            var deliverOrder = deliverOrders.Count > 0 ? deliverOrders[0] : null;
            if (deliverOrder != null)
            {
                var deliverOrderItemsItems = GetDeliverOrderItems(deliverOrder.Id);
                deliverOrderItemsItems.ForEach(x =>
                {
                    x.Item = GetItem(x.ItemId);
                    x.Unit = GetUnit(x.UnitId);
                    x.Batch = GetBatch(x.BatchId);
                    x.ItemName = x.Item.Name;
                    x.BatchName = x.Batch.Name;
                });
                deliverOrder.DeliverOrderItems = deliverOrderItemsItems;
            }
            return deliverOrder;
        }
        
        public override async Task<bool> DeleteAsync(int Id)
        {
            try
            {
                var deliverOrder = await _context.DeliverOrders.FindAsync(Id);
                if (deliverOrder == null)
                {
                    return false;
                }
                deliverOrder.IsDelete = true;
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