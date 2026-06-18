using System;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Fdsd.Infrastructure.Persistence;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly FdsdDbContext _dbContext;
    private IDbContextTransaction? _currentTransaction;

    public EfUnitOfWork(FdsdDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _dbContext.Dispose();
    }
}