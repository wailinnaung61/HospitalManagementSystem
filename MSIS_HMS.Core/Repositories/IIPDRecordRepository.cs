using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;

namespace MSIS_HMS.Core.Repositories
{
    public interface IIPDRecordRepository : IRepository<IPDRecord>
    {
        List<IPDRecord> GetAll(int? BranchId = null, int? IPDRecordId = null, string Status = null, int? PaymentType = null, int? BedId = null, int? RoomId = null, string VoucherNo = null, int? TreatmentProcess = null, DateTime? DOA = null, DateTime? DODC = null, DateTime? StartDate = null, DateTime? EndDate = null);
        List<Room> GetAvailableRoomsandBeds(int DepartmentId);
        IPDRecord GetIPDSingleRecord(int IPDRecordId);
        decimal GetIncomeForIPD(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null);
        List<IPDRecordForDashboardDTO> GetIPDRecordForDashboard(int? BranchId = null);
    }

}
