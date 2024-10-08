using System.Linq.Expressions;

namespace Template.Core.Repositories.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity> CreateAsync(TEntity entity);
    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? predicate = null, bool isTracked = true);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<TEntity?> GetByIdAsync(object id);
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = true);
    void Delete(TEntity entity);
}