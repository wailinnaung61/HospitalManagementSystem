using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;

namespace MSIS_HMS.Core.Repositories
{
    public interface IIPDAllotmentRepository : IRepository<IPDAllotment>
    {
        public List<IPDAllotment> GetAll(int IPDRecordId);
        public IPDAllotment GetIPDRecord(int IPDRecordId);
        public IPDAllotment GetIPDAllotment(int? AllotmentId);
    }

}
