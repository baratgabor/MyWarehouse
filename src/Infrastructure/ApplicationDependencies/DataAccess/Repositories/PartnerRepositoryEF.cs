using AutoMapper;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories;
using MyWarehouse.Domain.Partners;
using MyWarehouse.Infrastructure.ApplicationDependencies.DataAccess.Repositories.Common;
using MyWarehouse.Infrastructure.Persistence.Context;
using System.Collections.Generic;
using System.Linq;

namespace MyWarehouse.Infrastructure.ApplicationDependencies.DataAccess.Repositories
{
    internal class PartnerRepositoryEF : RepositoryBaseEF<Partner>, IPartnerRepository
    {
        protected override IQueryable<Partner> BaseQuery
            => _context.Partners;

        public PartnerRepositoryEF(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        { }

        public override void Remove(Partner entityToDelete)
        {
            _context.Remove(entityToDelete);
        }

        public override void RemoveRange(IEnumerable<Partner> entitiesToDelete)
        {
            foreach (var e in entitiesToDelete)
                Remove(e);
        }
    }
}