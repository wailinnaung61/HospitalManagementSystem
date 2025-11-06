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
    public class IPDOrderServiceRepository:Repository<IPDOrderService>,IIPDOrderServiceRepository
    {
        public IPDOrderServiceRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override IPDOrderService Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetIPDRecords", new Dictionary<string, object>() { { "IPDRecordId", Id } });
            var iPDOrderServices = ds.Tables[0].ToList<IPDOrderService>();
            return iPDOrderServices.Count > 0 ? iPDOrderServices[0] : null;
        }
        public List<IPDOrderService> GetIPDOrderServiceByIPDRecordId(int? Id,int? IPDRecordId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetIPDOrderService", new Dictionary<string, object>() {
                 { "IPDRecordId", IPDRecordId } });
            var iPDOrderServices = ds.Tables[0].ToList<IPDOrderService>();
            return iPDOrderServices;

        }
    }
}
