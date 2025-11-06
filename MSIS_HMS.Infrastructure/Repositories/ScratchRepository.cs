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
    public class ScratchRepository:Repository<Scratch> ,IScratchRepository
    {
        public ScratchRepository(ApplicationDbContext context,IConfigService configService):base(context,configService)
        {

        }
        public override List<Scratch> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetScratches");
            var scratches = ds.Tables[0].ToList<Scratch>();
            return scratches;
        }
        public override List<Scratch> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetScratches", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var scratches = ds.Tables[0].ToList<Scratch>();
            return scratches;
        }
        public List<Scratch> GetAll(int? BranchId, int? ScratchId, int? WarehouseId, int? ItemId,DateTime? StartDate = null, DateTime? EndDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetScratches", new Dictionary<string, object>()
            {
                { "BranchId", BranchId },
                { "ScratchId", ScratchId },
                { "WarehouseId", WarehouseId },
                { "ItemId", ItemId },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
            });
            var scratches = ds.Tables[0].ToList<Scratch>();
            return scratches;
        }
        public List<ScratchItem> GetScratchItems(int ScratchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetScratchItems", new Dictionary<string, object>() { { "ScratchId", ScratchId } });
            var scratchItems = ds.Tables[0].ToList<ScratchItem>();
            return scratchItems;
        }
        public List<WarehouseItem> GetScratchItemsForUpdate(int ScratchId)
        {
            var deliverOrder = Get(ScratchId);

            return GetScratchItems(ScratchId).Select(x => new WarehouseItem
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
        public override Scratch Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetScratches", new Dictionary<string, object>() { { "ScratchId", Id } });
            var scratches = ds.Tables[0].ToList<Scratch>();
            var scratche = scratches.Count > 0 ? scratches[0] : null;
            if (scratche != null)
            {
                var scratchItems = GetScratchItems(scratche.Id);
                scratchItems.ForEach(x =>
                {
                    x.Item = GetItem(x.ItemId);
                    x.Unit = GetUnit(x.UnitId);
                    x.Batch = GetBatch(x.BatchId);
                    x.ItemName = x.Item.Name;
                    x.BatchName = x.Batch.Name;
                });
                scratche.ScratchItems = scratchItems;
            }
            return scratche;
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
        public override async Task<bool> DeleteAsync(int Id)
        {
            try
            {
                var deliverOrder = await _context.Scratches.FindAsync(Id);
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
