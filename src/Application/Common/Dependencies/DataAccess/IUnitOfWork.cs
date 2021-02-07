using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories;
using System;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Common.Dependencies.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        public IPartnerRepository Partners { get; }
        public IProductRepository Products { get; }
        public ITransactionRepository Transactions { get; }

        public Task SaveChanges();
    }
}
