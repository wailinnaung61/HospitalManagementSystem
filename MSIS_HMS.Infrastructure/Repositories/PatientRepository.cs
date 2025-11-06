using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Patient> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPatients");
            var patients = ds.Tables[0].ToList<Patient>();
            return patients;
        }
        public override List<Patient> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPatients", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var patients = ds.Tables[0].ToList<Patient>();
            return patients;
        }
        public List<Patient> GetAll(int? BranchId = null, int? PatientId = null, DateTime? StartRegDate = null, DateTime? EndRegDate = null, string RegNo = null, string Name = null, string NRC = null, string Guardian = null, DateTime? DateOfBirth = null, string Phone = null, string BloodType = null,string Code=null,int? ReferrerId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPatients", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "PatientId", PatientId },
                { "StartRegDate", StartRegDate },
                { "EndRegDate", EndRegDate },
                { "RegNo", RegNo },
                { "Name", Name },
                { "NRC", NRC },
                { "Guardian", Guardian },
                { "DateOfBirth", DateOfBirth },
                { "Phone", Phone },
                { "BloodType", BloodType },
                { "ReferrerId", ReferrerId },
                {"BarOrQRCode",Code}
            });
            var patients = ds.Tables[0].ToList<Patient>();
            return patients;
        }
        public override Patient Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPatients", new Dictionary<string, object>() { { "PatientId", Id } });
            var patients = ds.Tables[0].ToList<Patient>();
            return patients.Count > 0 ? patients[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var patient = await _context.Patients.FindAsync(Id);
                    if (patient == null)
                    {
                        return false;
                    }
                    patient.IsDelete = true;
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