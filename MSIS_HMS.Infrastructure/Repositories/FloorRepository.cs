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
    public class FloorRepository :Repository<Floor>, IFloorRepository
    {
        public FloorRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Floor> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetFloor");
            var floor = ds.Tables[0].ToList<Floor>();
            return floor;
        }
        public override List<Floor> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetFloor", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var floor = ds.Tables[0].ToList<Floor>();
            return floor;
        }
        public List<Floor> GetAll(int? BranchId = null, int? FloorId = null,string Name = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetFloor", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "FloorId", FloorId },
                { "Name", Name },
               
            });
            var floor = ds.Tables[0].ToList<Floor>();
            return floor;
        }
        public override Floor Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetFloor", new Dictionary<string, object>() { { "FloorId", Id } });
            var floor = ds.Tables[0].ToList<Floor>();
            return floor.Count > 0 ? floor[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var floor = await _context.Floors.FindAsync(Id);
                    if (floor == null)
                    {
                        return false;
                    }
                    floor.IsDelete = true;
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