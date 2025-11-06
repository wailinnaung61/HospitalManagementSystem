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
    public class VisitTypeRepository : Repository<VisitType>,IVisitTypeRepository
    {
        public VisitTypeRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
            
        }
        public override List<VisitType> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetVisitTypes");
            var visittypes = ds.Tables[0].ToList<VisitType>();
            return visittypes;
        }
        public override List<VisitType> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetVisitTypes", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var visittypes = ds.Tables[0].ToList<VisitType>();
            return visittypes;
        }
        public override VisitType Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetVisitTypes", new Dictionary<string, object>() { { "VisitTypeId", Id } });
            var visitTypes = ds.Tables[0].ToList<VisitType>();
            return visitTypes.Count > 0 ? visitTypes[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var visitType = await _context.VisitTypes.FindAsync(Id);
                    if (visitType == null)
                    {
                        return false;
                    }
                    visitType.IsDelete = true;
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