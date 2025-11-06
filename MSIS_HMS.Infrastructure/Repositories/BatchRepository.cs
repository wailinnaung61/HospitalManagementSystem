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

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class BatchRepository : Repository<Batch>, IBatchRepository
    {
        public BatchRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Batch> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBatches");
            var batches = ds.Tables[0].ToList<Batch>();
            return batches;
        }
        public override List<Batch> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBatches", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var batches = ds.Tables[0].ToList<Batch>();
            return batches;
        }
        public List<Batch> GetAll(int? BranchId, int? ItemId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBatches", new Dictionary<string, object>() { { "BranchId", BranchId }, { "ItemId", ItemId } });
            var batches = ds.Tables[0].ToList<Batch>();
            return batches;
        }
        public List<Batch> GetAll(int? BranchId, int? ItemId, int? BatchId = null, string BatchName = null, string BatchCode = null, string BatchNumber = null, DateTime? ExpiryDate = null, DateTime? StartExpiryDate = null, DateTime? EndExpiryDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBatches", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "ItemId", ItemId },
                { "BatchId", BatchId },
                { "BatchName", BatchName },
                { "BatchCode", BatchCode },
                { "BatchNumber", BatchNumber },
                { "ExpiryDate", ExpiryDate },
                { "StartExpiryDate", StartExpiryDate },
                { "EndExpiryDate", EndExpiryDate },
            });
            var batches = ds.Tables[0].ToList<Batch>();
            return batches;
        }
        public override Batch Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBatches", new Dictionary<string, object>() { { "BatchId", Id } });
            var batches = ds.Tables[0].ToList<Batch>();
            return batches.Count > 0 ? batches[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var batch = await _context.Batches.FindAsync(Id);
                    if (batch == null)
                    {
                        return false;
                    }
                    batch.IsDelete = true;
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