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
    public class OperationTypeRepository : Repository<OperationType>, IOperationTypeRepository
    {
        public OperationTypeRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<OperationType> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationTypes");
            var operationtypes = ds.Tables[0].ToList<OperationType>();
            return operationtypes;
        }
        public override List<OperationType> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationTypes", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var operationtypes = ds.Tables[0].ToList<OperationType>();
            return operationtypes;
        }
        public override OperationType Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationTypes", new Dictionary<string, object>() { { "OperationTypeId", Id } });
            var operationTypes = ds.Tables[0].ToList<OperationType>();
            return operationTypes.Count > 0 ? operationTypes[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var operationType = await _context.OperationTypes.FindAsync(Id);
                    if (operationType == null)
                    {
                        return false;
                    }
                    operationType.IsDelete = true;
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