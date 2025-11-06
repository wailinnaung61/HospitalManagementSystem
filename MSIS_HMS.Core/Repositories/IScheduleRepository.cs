using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IScheduleRepository : IRepository<Schedule>
    {
        List<Schedule> GetAll(int? BranchId, int? DoctorId = null);
        List<Schedule> GetAll(int? BranchId = null, int? ScheduleId = null, int? DepartmentId = null, int? DepartmentType = null, int? DoctorId = null, int? DayOfWeek = null, TimeSpan? Time = null, TimeSpan? FromTime = null, TimeSpan? ToTime = null);
    }
}