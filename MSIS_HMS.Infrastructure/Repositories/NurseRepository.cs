using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class NurseRepository : Repository<Nurse>, INurseRepository
    {
        public NurseRepository(ApplicationDbContext context, IConfigService config) : base(context, config)
        {
        }
        public override List<Nurse> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetNurses");
            var nurses = ds.Tables[0].ToList<Nurse>();
            return nurses;
        }
        public override List<Nurse> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetNurses", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var nurses = ds.Tables[0].ToList<Nurse>();
            return nurses;
        }
        public override Nurse Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetNurses", new Dictionary<string, object>() { { "NurseId", Id } });
            var nurses = ds.Tables[0].ToList<Nurse>();
            return nurses.Count > 0 ? nurses[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var nurse = await _context.Nurses.FindAsync(Id);
                    if (nurse == null)
                    {
                        return false;
                    }
                    nurse.IsDelete = true;
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