using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class IPDAllotmentRepository : Repository<IPDAllotment>, IIPDAllotmentRepository
    {
        public IPDAllotmentRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public IPDAllotment GetIPDRecord(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAllotments", new Dictionary<string, object>() { { "IPDRecordId", Id } });
            var allotments = ds.Tables[0].ToList<IPDAllotment>();
     
            return allotments.Count > 0 ? allotments[0] : null;
        }
        public  IPDAllotment GetIPDAllotment(int? Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAllotments", new Dictionary<string, object>() { { "IPDAllotmentId", Id } });
            var allotments = ds.Tables[0].ToList<IPDAllotment>();

            return allotments.Count > 0 ? allotments[0] : null;
        }

        public List<IPDAllotment> GetAll(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetAllotments", new Dictionary<string, object>() { { "IPDRecordId", Id } });
            var allotments = ds.Tables[0].ToList<IPDAllotment>();
            return allotments;
        }
    }
}
