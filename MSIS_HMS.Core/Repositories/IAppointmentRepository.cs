using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;

namespace MSIS_HMS.Core.Repositories
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        List<Appointment> GetAppointmentByDoctorId(int? DoctorId);
        List<Appointment> GetAppointmentByDateTime(DateTime startDate, DateTime endDate);
    }
}