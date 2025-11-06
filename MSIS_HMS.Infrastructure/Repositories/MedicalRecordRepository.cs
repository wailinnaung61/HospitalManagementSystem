using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class MedicalRecordRepository : Repository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<MedicalRecord> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetMedicalRecords");
            var medicalRecords = ds.Tables[0].ToList<MedicalRecord>();
            medicalRecords.ForEach(x => x.PatientSymptoms = GetPatientSymptoms(x.Id));
            return medicalRecords;
        }
        public override List<MedicalRecord> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetMedicalRecords", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var medicalRecords = ds.Tables[0].ToList<MedicalRecord>();
            medicalRecords.ForEach(x => x.PatientSymptoms = GetPatientSymptoms(x.Id));
            return medicalRecords;
        }
        public List<MedicalRecord> GetAll(int? BranchId = null, int? MedicalRecordId = null, int? VisitId = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null, int? PatientId = null, int? DoctorId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetMedicalRecords", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "MedicalRecordId", MedicalRecordId },
                { "VisitId", VisitId },
                { "PatientId", PatientId },
                { "DoctorId", DoctorId },
                { "Date", Date },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
            });
            var medicalRecords = ds.Tables[0].ToList<MedicalRecord>();
            medicalRecords.ForEach(x => x.PatientSymptoms = GetPatientSymptoms(x.Id));
            medicalRecords.ForEach(x => x.PatientDiagnostics = GetPatientDiagnostics(x.Id));
            medicalRecords.ForEach(x => x.Prescriptions = GetPrescriptions(x.Id));
            return medicalRecords;
        }
        public override MedicalRecord Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetMedicalRecords", new Dictionary<string, object>() { { "MedicalRecordId", Id } });
            var medicalRecords = ds.Tables[0].ToList<MedicalRecord>();
            medicalRecords.ForEach(x => {
                x.PatientSymptoms = GetPatientSymptoms(x.Id);
                x.PatientDiagnostics = GetPatientDiagnostics(x.Id);
                x.Prescriptions = GetPrescriptions(x.Id);
            });
            if (medicalRecords.Count > 0)
            {
                if (medicalRecords[0].Gender == GenderEnum.Gender.Female)
                    medicalRecords[0].GenderStr = GenderEnum.Gender.Female.ToDescription();
                else if (medicalRecords[0].Gender == GenderEnum.Gender.Male)
                    medicalRecords[0].GenderStr = GenderEnum.Gender.Male.ToDescription();
                else
                    medicalRecords[0].GenderStr = GenderEnum.Gender.Other.ToDescription();

            }
            return medicalRecords.Count > 0 ? medicalRecords[0] : null;
        }
        private List<PatientSymptom> GetPatientSymptoms(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPatientSymptoms", new Dictionary<string, object>() { { "MedicalRecordId", Id } });
            return ds.Tables[0].ToList<PatientSymptom>();
        }
        private List<PatientDiagnostic> GetPatientDiagnostics(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPatientDiagnostics", new Dictionary<string, object>() { { "MedicalRecordId", Id } });
            return ds.Tables[0].ToList<PatientDiagnostic>();
        }
        private List<Prescription> GetPrescriptions(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPrescriptions", new Dictionary<string, object>() { { "MedicalRecordId", Id } });
            return ds.Tables[0].ToList<Prescription>();
        }
        public override async Task<MedicalRecord> UpdateAsync(MedicalRecord medicalRecord)
        {
            if (medicalRecord != null)
            {
                medicalRecord.PatientSymptoms.ToList().ForEach(x => x.MedicalRecordId = medicalRecord.Id);
                medicalRecord.PatientDiagnostics.ToList().ForEach(x => x.MedicalRecordId = medicalRecord.Id);
                medicalRecord.Prescriptions.ToList().ForEach(x => x.MedicalRecordId = medicalRecord.Id);
                var existingMedicalRecord = _context.MedicalRecords
                    .Where(p => p.Id == medicalRecord.Id)
                    .Include(p => p.PatientSymptoms)
                    .Include(x => x.PatientDiagnostics)
                    .Include(x => x.Prescriptions)
                    .SingleOrDefault();

                if (existingMedicalRecord != null)
                {
                    // Update parent
                    _context.Entry(existingMedicalRecord).CurrentValues.SetValues(medicalRecord);

                    // Delete children
                    _context.PatientSymptoms.RemoveRange(existingMedicalRecord.PatientSymptoms);
                    _context.PatientDiagnostics.RemoveRange(existingMedicalRecord.PatientDiagnostics);
                    _context.Prescriptions.RemoveRange(existingMedicalRecord.Prescriptions);

                    // Insert children
                    existingMedicalRecord.PatientSymptoms = medicalRecord.PatientSymptoms;
                    existingMedicalRecord.PatientDiagnostics = medicalRecord.PatientDiagnostics;
                    existingMedicalRecord.Prescriptions = medicalRecord.Prescriptions;

                    await _context.SaveChangesAsync();
                    return medicalRecord;
                }
            }
            return null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var medicalRecord = await _context.MedicalRecords.FindAsync(Id);
                    if (medicalRecord == null)
                    {
                        return false;
                    }
                    medicalRecord.IsDelete = true;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (DbException e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                }
            }
            return false;
        }
    }
}