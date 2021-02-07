using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories;
using MyWarehouse.Infrastructure.Persistence.Context;
using System.Threading.Tasks;

namespace MyWarehouse.Infrastructure.CoreDependencies.DataAccess
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public IPartnerRepository Partners { get; }
        public IProductRepository Products { get; }
        public ITransactionRepository Transactions { get; }

        public UnitOfWork(ApplicationDbContext dbContext, IPartnerRepository partners, IProductRepository products, ITransactionRepository transactions)
        {
            _dbContext = dbContext;
            Partners = partners;
            Products = products;
            Transactions = transactions;
        }

        public void Dispose()
            => _dbContext.Dispose();

        public Task SaveChanges()
            => _dbContext.SaveChangesAsync();
    }
}
