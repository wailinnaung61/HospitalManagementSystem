using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IWardRepository : IRepository<Ward>
    {
        List<Ward> GetAll(int? WardId=null,string Name=null,int? DepartmentId=null,int? FloorId=null);
    }
}