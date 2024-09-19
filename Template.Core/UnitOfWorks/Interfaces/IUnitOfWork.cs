using Template.Core.Repositories.Interfaces;

namespace Template.Core.UnitOfWorks.Interfaces;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task SaveChangesAsync();
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
}