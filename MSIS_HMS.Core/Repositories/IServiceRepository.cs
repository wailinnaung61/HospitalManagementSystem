using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;

namespace MSIS_HMS.Core.Repositories
{
    public interface IServiceRepository : IRepository<Service>
    {
        List<ServiceDTO> GetServiceFromOrder(int? BranchId = null, DateTime? FromDate = null, DateTime? ToDate = null, int? OutletId = null);
    }
}