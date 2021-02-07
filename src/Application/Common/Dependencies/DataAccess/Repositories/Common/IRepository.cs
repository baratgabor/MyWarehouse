using MyWarehouse.Application.Common.Mapping;
using MyWarehouse.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        /// <summary>
        /// Returns the entity corresponding to the given ID, or default if not found.
        /// </summary>
        Task<TEntity> GetByIdAsync(int id);

        Task<IEnumerable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> filter, bool readOnly = false);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        void StartTracking(TEntity entity);

        /// <summary>
        /// Finds the entity of the given Id, and returns it mapped to the specified mappable type, or returns default if not found.
        /// </summary>
        Task<TDto> GetProjectedAsync<TDto>(int id, bool readOnly = false) where TDto : IMapFrom<TEntity>;

        /// <summary>
        /// Finds the list of entities corresponding to the provided query, and returns them mapped to the specified mappable type.
        /// </summary>
        Task<IListResponseModel<TDto>> GetProjectedListAsync<TDto>(ListQueryModel<TDto> model, Expression<Func<TEntity, bool>> additionalFilter = null, bool readOnly = false) where TDto : IMapFrom<TEntity>;
    }
}