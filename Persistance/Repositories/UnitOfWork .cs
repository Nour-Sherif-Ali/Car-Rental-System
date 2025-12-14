using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Persistance.Data;

namespace Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CarRentalDbContext _context;
        private readonly Dictionary<Type, object> _repositories;

        public UnitOfWork(CarRentalDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(
                    repositoryType.MakeGenericType(typeof(T)), _context);
                _repositories.Add(type, repositoryInstance!);
            }
            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
