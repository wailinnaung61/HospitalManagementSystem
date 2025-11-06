using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface ILabResultRepository : IRepository<LabResult>
    {
        List<LabResult> GetAll(int? BranchId = null, int? LabResultId = null, int? PatientId = null, string ResultNo = null, int? TechnicianId = null, int? ConsultantId = null, bool? IsCompleted = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null, DateTime? CompletedDate = null, DateTime? StartCompletedDate = null, DateTime? EndCompletedDate = null, int? LabTestId = null);
    }
}