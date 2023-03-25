using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories;

namespace MyWarehouse.Application.Common.Dependencies.DataAccess;

public interface IUnitOfWork : IDisposable
{
    public IPartnerRepository Partners { get; }
    public IProductRepository Products { get; }
    public ITransactionRepository Transactions { get; }
    bool HasActiveTransaction { get; }

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    public Task SaveChanges();
}
