using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Repositories.Base;
using MSIS_HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MSIS_HMS.Infrastructure.Interfaces;

namespace MSIS_HMS.Infrastructure.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly string _connectionString;

        public Repository(ApplicationDbContext context, IConfigService configService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _connectionString = configService.GetConnectionString();
        }

        public virtual List<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public virtual List<T> GetAll(int? BranchId)
        {
            throw new NotImplementedException();
        }

        public virtual T Get(int Id)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }   

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
            _context.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual Task<bool> DeleteAsync(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
