using System;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    // IDisposable so that we can use the Dispose() method
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        Task<int> Complete();  // return the number of changes to our database (217.)
    }
}
