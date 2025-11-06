using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System.Collections.Generic;

namespace MSIS_HMS.Core.Repositories
{
    public interface IBedRepository : IRepository<Bed>
    {
        List<Bed> GetAll(int? BedId = null, int? BedTypeId = null, string BedStatus = null,int? RoomId = null,string BedNo = null);

    }
}