using Microsoft.EntityFrameworkCore.Storage;
using Template.Core.Repositories;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database;

namespace Template.Core.UnitOfWorks;

public class UnitOfWork(DatabaseContext dbContext) : IUnitOfWork
{
    private readonly DatabaseContext _dbContext = dbContext;
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync()
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("A transaction has already been started.");
        }

        _transaction = await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("A transaction has not been started.");
        }

        try
        {
            await _transaction.CommitAsync();
            _transaction.Dispose();
            _transaction = null;
        }
        catch (Exception)
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync();
            }

            throw;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        return new GenericRepository<TEntity>(_dbContext);
    }
}