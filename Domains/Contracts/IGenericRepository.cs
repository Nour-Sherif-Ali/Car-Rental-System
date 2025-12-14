using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec);
        Task<int> CountAsync(ISpecification<T> spec);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
