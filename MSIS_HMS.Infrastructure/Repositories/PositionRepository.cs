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

namespace MSIS_HMS.Infrastructure.Repositories.Base
{
    public class PositionRepository : Repository<Position>, IPositionRepository
    {
        public PositionRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override List<Position> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPosition");
            var positions = ds.Tables[0].ToList<Position>();
            return positions;
        }
        public List<Position> GetAll( int? PositionId = null, string Name = null, string Code = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPosition", new Dictionary<string, object>() {
                { "PositionId", PositionId },
                { "Name", Name },
                { "Code", Code },
            });
            var positions = ds.Tables[0].ToList<Position>();
            return positions;
        }
        public override Position Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPosition", new Dictionary<string, object>() { { "PositionId", Id } });
            var positions = ds.Tables[0].ToList<Position>();
            return positions.Count > 0 ? positions[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var position = await _context.Positions.FindAsync(Id);
                    if (position == null)
                    {
                        return false;
                    }
                    position.IsDelete = true;
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