using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Template.Core.UnitOfWorks;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database;

namespace Template.Test;

public class BaseTest
{
    protected readonly DatabaseContext _dbContext;
    protected readonly IUnitOfWork _unitOfWork;

    public BaseTest()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new DatabaseContext(options);
        _dbContext.Database.BeginTransaction(); 
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        _unitOfWork = new UnitOfWork(_dbContext);
    }
}