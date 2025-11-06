using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class PatientResultImageRepository:Repository<PatientResultImage>,IPatientResultImageRepository
    {
        public PatientResultImageRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public   PatientImageDTO Get(int Id)
        {
            List<PatientImageDTO> lstImg = new List<PatientImageDTO>();
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPatientResultImage", new Dictionary<string, object>() { { "PatientResultImageId", Id } });
            var patients = ds.Tables[0].ToList<PatientResultImage>();
            foreach(var p in patients)
            {
                PatientImageDTO patientImageDTO = new PatientImageDTO();
                List<string> fileName = new List<string>();
                List<string> attachPath = new List<string>();
                patientImageDTO.Id = p.Id;
                fileName.Add(p.Name);
                patientImageDTO.Name = fileName;
                attachPath.Add(p.AttachmentPath);
                patientImageDTO.AttachmentPath=attachPath;
                lstImg.Add(patientImageDTO);
            }
            return lstImg.Count > 0 ? lstImg[0] : null;
        }
        public List<PatientResultImage> GetAll(int? BranchId = null, int? PatientId = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPatientResultImage", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "PatientId", PatientId },
                { "StartDate", StartDate },
                { "EndDate", EndDate }
              
            });
            var patientResultImages = ds.Tables[0].ToList<PatientResultImage>();
            return patientResultImages;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var patient = await _context.patientResultImages.FindAsync(Id);
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
