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
    public class OperationRoomRepository : Repository<OperationRoom>, IOperationRoomRepository
    {
        public OperationRoomRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<OperationRoom> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationRooms");
            var oproom = ds.Tables[0].ToList<OperationRoom>();
            return oproom;
        }

        public List<OperationRoom> GetAll(int? OperationRoomId = null, string RoomStatus = null, decimal? Price = null, int? WardId = null, string RoomNo = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationRooms", new Dictionary<string, object>() {
               {"OperationRoomId",OperationRoomId},
                {"Status",RoomStatus },
                {"WardId",WardId },
                {"RoomNo",RoomNo}
            });
            var operationRoom = ds.Tables[0].ToList<OperationRoom>();
           return operationRoom;
        }
        public override List<OperationRoom> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationRooms", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var operationRooms = ds.Tables[0].ToList<OperationRoom>();
            return operationRooms;
        }
        public override OperationRoom Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationRooms", new Dictionary<string, object>() { { "OperationRoomId", Id } });
            var operationRoom = ds.Tables[0].ToList<OperationRoom>();
            return operationRoom.Count > 0 ? operationRoom[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var ward = await _context.OperationRooms.FindAsync(Id);
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