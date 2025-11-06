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
    public class RoomTypeRepository : Repository<RoomType>, IRoomTypeRepository
    {
        public RoomTypeRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override List<RoomType> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetRoomType");
            var roomTypes = ds.Tables[0].ToList<RoomType>();
            return roomTypes;
        }
       
        public override RoomType Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetRoomType", new Dictionary<string, object>() { { "RoomTypeId", Id } });
            var roomTypes = ds.Tables[0].ToList<RoomType>();
            return roomTypes.Count > 0 ? roomTypes[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var roomTypes = await _context.RoomTypes.FindAsync(Id);
                    if (roomTypes == null)
                    {
                        return false;
                    }
                    roomTypes.IsDelete = true;
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