using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Template.Core.Repositories.Interfaces;
using Template.Database;

namespace Template.Core.Repositories;

public class GenericRepository<TEntity>(DatabaseContext dbContext) : IGenericRepository<TEntity> where TEntity : class
{
    private readonly DatabaseContext _dbContext = dbContext;
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? predicate)
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(object id)
    {
        TEntity? entity = await _dbSet.FindAsync(id);
        return entity;
    }

    public void Update(TEntity entity)
    {
        _dbSet.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(TEntity entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
    }
}