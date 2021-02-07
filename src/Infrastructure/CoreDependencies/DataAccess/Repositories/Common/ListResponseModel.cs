using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;
using System;
using System.Collections.Generic;

namespace MyWarehouse.Infrastructure.CoreDependencies.DataAccess.Repositories.Common
{
    public class ListResponseModel<TDto> : IListResponseModel<TDto>
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }

        public int PageCount { get; private set; }
        public int RowCount { get; private set; }

        public string ActiveFilter { get; private set; }
        public string ActiveOrderBy { get; private set; }

        public int FirstRowOnPage => RowCount <= 0 ? 0 : ((PageIndex - 1) * PageSize) + 1;
        public int LastRowOnPage => Math.Min(PageIndex * PageSize, RowCount);

        public IEnumerable<TDto> Results { get; set; } = new List<TDto>();

        public ListResponseModel(ListQueryModel<TDto> queryModel, int rowCount, IEnumerable<TDto> results)
        {
            Results = results;

            PageIndex = queryModel.PageIndex;
            PageSize = queryModel.PageSize;
            ActiveOrderBy = queryModel.OrderBy;
            ActiveFilter = queryModel.Filter;
            RowCount = rowCount;
            PageCount = (int)Math.Ceiling((double)rowCount / PageSize);
        }
    }
}