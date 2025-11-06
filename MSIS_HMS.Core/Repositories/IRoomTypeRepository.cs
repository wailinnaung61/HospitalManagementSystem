using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IRoomTypeRepository : IRepository<RoomType>
    {
        List<RoomType> GetAll();
    }
}