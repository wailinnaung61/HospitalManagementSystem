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
using System.Text;
using System.Threading.Tasks;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class WardRepository : Repository<Ward>, IWardRepository
    {
        public WardRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Ward> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWards");
            var ward = ds.Tables[0].ToList<Ward>();
            return ward;
        }
       
        public List<Ward> GetAll(int? WardId = null, string Name = null, int? DepartmentId = null, int? FloorId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWards", new Dictionary<string, object>() {
                { "WardId", WardId },
                { "Name", Name },
                { "DepartmentId", DepartmentId },
                { "FloorId", FloorId },

            });
            var ward = ds.Tables[0].ToList<Ward>();
            return ward;
        }
        public override Ward Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetWards", new Dictionary<string, object>() { { "WardId", Id } });
            var ward = ds.Tables[0].ToList<Ward>();
            return ward.Count > 0 ? ward[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var ward = await _context.Wards.FindAsync(Id);
                    if (ward == null)
                    {
                        return false;
                    }
                    ward.IsDelete = true;
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