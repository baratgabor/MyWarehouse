using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories;
using MyWarehouse.Domain.Transactions;
using MyWarehouse.Infrastructure.ApplicationDependencies.DataAccess.Repositories.Common;
using MyWarehouse.Infrastructure.Persistence.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWarehouse.Infrastructure.ApplicationDependencies.DataAccess.Repositories
{
    internal class TransactionRepositoryEF : RepositoryBaseEF<Transaction>, ITransactionRepository
    {
        protected override IQueryable<Transaction> BaseQuery
            => _context.Transactions.Include(e => e.TransactionLines)
                // This is a crude way to make sure soft-deleted Partners and Products won't cause referencing Transactions to be hidden.
                // Currently there is no way to disable only certain query filters. Fortunately, though, there are no side-effects in this case, because transactions cannot be deleted. 
                // This solution also breaks the encapsulation of soft-delete logic in DbContext...
                // Hopefully they'll soon extend the global query filter functionality.
                .IgnoreQueryFilters();

        public TransactionRepositoryEF(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        { }

        public override void Remove(Transaction entityToDelete)
            => _set.Remove(entityToDelete);

        public override void RemoveRange(IEnumerable<Transaction> entitiesToDelete)
            => _set.RemoveRange(entitiesToDelete);

        public async Task<Transaction> GetEntireTransaction(int id)
            => await BaseQuery
                .Include(x => x.Partner)
                .Include(x => x.TransactionLines)
                    .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
    }
}