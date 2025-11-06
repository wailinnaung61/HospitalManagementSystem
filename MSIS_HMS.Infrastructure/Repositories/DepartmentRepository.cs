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
    public class DepartmentRepository : Repository<Department>,IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context, IConfigService config) : base(context, config)
        {            
        }
        public override List<Department> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDepartments");
            var departments = ds.Tables[0].ToList<Department>();
            return departments;
        }
        public override List<Department> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDepartments", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var departments = ds.Tables[0].ToList<Department>();
            return departments;
        }
        public override Department Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDepartments", new Dictionary<string, object>() { { "DepartmentId", Id } });
            var departments = ds.Tables[0].ToList<Department>();
            return departments.Count > 0 ? departments[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var department = await _context.Departments.FindAsync(Id);
                    if (department == null)
                    {
                        return false;
                    }
                    department.IsDelete = true;
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