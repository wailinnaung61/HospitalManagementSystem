﻿using MSIS_HMS.Core.Entities;
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
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        public RoomRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Room> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetRooms");
            var room = ds.Tables[0].ToList<Room>();
            return room;
        }
        
        public List<Room> GetAll(int? RoomId = null, int? RoomTypeId = null, string RoomStatus = null, decimal? Price = null, int? WardId = null,string RoomNo = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetRooms", new Dictionary<string, object>() {
               {"RoomId",RoomId},
                {"RoomTypeId",RoomTypeId },
                {"Status",RoomStatus },
                {"WardId",WardId },
                {"RoomNo",RoomNo}
            });
            var room = ds.Tables[0].ToList<Room>();
            return room;
        }
        public override Room Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetRooms", new Dictionary<string, object>() { { "RoomId", Id } });
            var room = ds.Tables[0].ToList<Room>();
            return room.Count > 0 ? room[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var ward = await _context.Rooms.FindAsync(Id);
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