using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        List<Room> GetAll(int? RoomId=null,int? RoomTypeId=null,string RoomStatus=null,decimal? Price=null,int? WardId=null,string RoomNo = null);
    }
}