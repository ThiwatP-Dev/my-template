using System.Linq.Expressions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Template.Core.Repositories.Interfaces;
using Template.Database;

namespace Template.Core.Repositories;

public class GenericRepository<TEntity>(DatabaseContext dbContext) : IGenericRepository<TEntity> where TEntity : class
{
    private readonly DatabaseContext _dbContext = dbContext;
    protected readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return entities;
    }

    public virtual async Task BulkCreateAsync(IEnumerable<TEntity> entities)
    {
        await _dbContext.BulkInsertAsync(entities);
    }

    public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? predicate = null, bool isTracked = true)
    {
        var query = isTracked ? _dbSet : _dbSet.AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return query;
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var query = _dbSet.AsNoTracking();

        if (predicate is not null)
        {
            return await query.AnyAsync(predicate);
        }

        return await query.AnyAsync();
    }

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var query = _dbSet.AsNoTracking();

        if (predicate is not null)
        {
            return await query.CountAsync(predicate);
        }

        return await query.CountAsync();
    }

    public async Task<TEntity?> GetByIdAsync(object id)
    {
        var entity = await _dbSet.FindAsync(id);

        return entity;
    }

    public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = true)
    {
        var query = isTracked ? _dbSet : _dbSet.AsNoTracking();
        var entity = await query.SingleOrDefaultAsync(predicate);
        return entity;
    }

    public virtual void Delete(TEntity entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
    }

    public virtual void DeleleRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            Delete(entity);
        }
    }
}