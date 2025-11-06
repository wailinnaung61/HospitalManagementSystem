using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IMedicalRecordRepository : IRepository<MedicalRecord>
    {
        List<MedicalRecord> GetAll(int? BranchId = null, int? MedicalRecordId = null, int? VisitId = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null, int? PatientId = null, int? DoctorId = null);
      
    }
}