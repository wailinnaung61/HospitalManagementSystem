using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IVisitRepository : IRepository<Visit>
    {
        List<Visit> GetAll(int? BranchId = null, int? VisitId = null, string VisitNo = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null, int? PatientId = null, int? DoctorId = null, int? VisitTypeId = null, VisitStatusEnum? Status = null);
        List<VisitPatientDTO> GetVisitPatient(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null, VisitStatusEnum? Status = null);
        List<decimal> GetCFFee(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null);
        List<DoctorHistoryDTO> GetDoctorHistory(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null, VisitStatusEnum? Status = null);
        List<CFFeeReportDTO> GetCFFeeReport(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null, int? DoctorId = null);
    }
}