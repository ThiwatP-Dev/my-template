using System.Linq.Expressions;

namespace Template.Core.Repositories.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity> CreateAsync(TEntity entity);
    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<TEntity?> GetByIdAsync(object id);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}