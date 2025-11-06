using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IPositionRepository : IRepository<Position>
    {
        List<Position> GetAll(int? PositionId=null,string Name=null,string Code=null);
    }
}