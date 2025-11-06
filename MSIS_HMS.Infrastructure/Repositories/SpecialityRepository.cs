using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class SpecialityRepository : Repository<Speciality>, ISpecialityRepository
    {
        public SpecialityRepository(ApplicationDbContext context, IConfigService config) : base(context, config)
        {
        }
        public override List<Speciality> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetSpecialities");
            var specialities = ds.Tables[0].ToList<Speciality>();
            return specialities;
        }
        public override List<Speciality> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetSpecialities", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var specialities = ds.Tables[0].ToList<Speciality>();
            return specialities;
        }
        public override Speciality Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetSpecialities", new Dictionary<string, object>() { { "SpecialityId", Id } });
            var specialities = ds.Tables[0].ToList<Speciality>();
            return specialities.Count > 0 ? specialities[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var speciality = await _context.Specialities.FindAsync(Id);
                    if (speciality == null)
                    {
                        return false;
                    }
                    speciality.IsDelete = true;
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