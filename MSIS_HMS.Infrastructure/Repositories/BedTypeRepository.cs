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
    public class BedTypeRepository : Repository<BedType>, IBedTypeRepository
    {
        public BedTypeRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override List<BedType> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBedType");
            var bedTypes = ds.Tables[0].ToList<BedType>();
            return bedTypes;
        }

        public override BedType Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBedType", new Dictionary<string, object>() { { "BedTypeId", Id } });
            var bedTypes = ds.Tables[0].ToList<BedType>();
            return bedTypes.Count > 0 ? bedTypes[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var bedTypes = await _context.BedTypes.FindAsync(Id);
                    if (bedTypes == null)
                    {
                        return false;
                    }
                    bedTypes.IsDelete = true;
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