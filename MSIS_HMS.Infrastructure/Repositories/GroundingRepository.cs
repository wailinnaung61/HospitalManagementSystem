using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class GroundingRepository:Repository<Grounding>,IGroundingRepository
    {
        public GroundingRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Grounding> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetGrounding");
            var groundings = ds.Tables[0].ToList<Grounding>();
            return groundings;
        }
        public List<Grounding> GetAll(int? BranchId = null, int? warehouseId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetGrounding", new Dictionary<string, object>()
            { { "BranchId", BranchId },
              { "WarehouseId", warehouseId },
              { "StartDate", fromDate },
              { "EndDate", toDate },
            });
            var groundings = ds.Tables[0].ToList<Grounding>();
            
            return groundings;
        }
        public override Grounding Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetGrounding", new Dictionary<string, object>()
            { {"GroundingId",Id}}); 
            var groundings = ds.Tables[0].ToList<Grounding>();
            var grounding = groundings.Count > 0 ? groundings[0] : null;
            grounding=GetGrounding(grounding);
            return grounding;
        }

        public Grounding GetGrounding(Grounding grounding)
        {
            if (grounding != null)
            {
                var groundingItems = GetGroundingItems(grounding.Id);
                groundingItems.ForEach(x =>
                {
                    x.Item = GetItem(x.ItemId);
                    x.Unit = GetUnit(x.UnitId);
                    x.Batch = GetBatch(x.BatchId);
                    x.ItemName = x.Item.Name;
                    x.BatchName = x.Batch.Name;
                });
                grounding.GroundingItems = groundingItems;
            }
            return grounding;
        }
        public List<GroundingItem> GetGroundingItems(int groundingId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetGroundingItem", new Dictionary<string, object>()
            {  {"GroundingId",groundingId} });
            var groundingItems = ds.Tables[0].ToList<GroundingItem>();

            return groundingItems;
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
    }
}
