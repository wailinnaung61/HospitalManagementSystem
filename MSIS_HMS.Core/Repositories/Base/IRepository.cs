using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MSIS_HMS.Core.Repositories.Base
{
    public interface IRepository<T> where T : Entity
    {
        List<T> GetAll();
        List<T> GetAll(int? BranchId);
        T Get(int Id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int Id);
    }
}
