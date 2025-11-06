using Microsoft.Extensions.Configuration;
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
using System.Threading.Tasks;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class BranchRepository : Repository<Branch>, IBranchRepository
    {
        public BranchRepository(ApplicationDbContext context, IConfigService config) : base(context, config)
        {
        }

        public override List<Branch> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBranches");
            var branches = ds.Tables[0].ToList<Branch>();
            return branches;
        }

        public override Branch Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBranches", new Dictionary<string, object>() { { "BranchId", Id } });
            var branches = ds.Tables[0].ToList<Branch>();
            return branches.Count() > 0 ? branches[0] : null;
        }
        public List<Branch> GetNonMainBranch(int MainBranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetNonBranches", new Dictionary<string, object>() { { "BranchId", MainBranchId } });
            var branches = ds.Tables[0].ToList<Branch>();
            return branches;

        }

        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var branch = await _context.Branches.FindAsync(Id);
                    if (branch == null)
                    {
                        return false;
                    }
                    branch.IsDelete = true;
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
