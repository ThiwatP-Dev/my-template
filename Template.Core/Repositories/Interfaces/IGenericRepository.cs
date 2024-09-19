using System.Linq.Expressions;

namespace Template.Core.Repositories.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity> CreateAsync(TEntity entity);
    Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<TEntity?> GetByIdAsync(object id);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}