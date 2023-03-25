using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;
using MyWarehouse.Domain.Transactions;

namespace MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<Transaction?> GetEntireTransaction(int id);
}
