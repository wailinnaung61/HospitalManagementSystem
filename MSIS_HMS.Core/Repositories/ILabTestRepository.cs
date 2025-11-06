using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface ILabTestRepository : IRepository<LabTest>
    {
        List<LabTest> GetAll(int? BranchId = null, int? LabTestId = null, string Name = null, string Code = null, LabCategoryEnum? Category = null, bool? IsLabReport = null);
        List<LabTestDetail> GetLabTestDetails(int Id);
        List<LabTestDTO> GetLabTestByLabOrderId(int labOrderId);
        List<LabTest> GetLabTestByOrderId(int? labOrderId = null);
    }
}