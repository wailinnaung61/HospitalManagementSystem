using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IOperationRoomRepository : IRepository<OperationRoom>
    {
        List<OperationRoom> GetAll(int? OperationRoomId = null, string RoomStatus = null, decimal? Price = null, int? WardId = null, string RoomNo = null);
    }
}