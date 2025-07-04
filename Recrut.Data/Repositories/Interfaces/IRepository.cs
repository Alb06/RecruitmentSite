﻿using System.Linq.Expressions;

namespace Recrut.Data.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetByAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>?> GetByIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<T>> GetAllAsync();
        Task CreateAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task<int> DeleteByIdsAsync(IEnumerable<int> entityIds);
        Task DeleteAsync(IEnumerable<T> entities);
    }
}
