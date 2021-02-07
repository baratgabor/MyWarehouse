using MediatR;
using MyWarehouse.Application.Common.Exceptions;
using System;

namespace MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common
{
    public class ListQueryModel<TDto> : IRequest<IListResponseModel<TDto>>
    {
        /// <summary>
        /// The index of the page to fetch.
        /// </summary>
        //[Range(1, int.MaxValue, ErrorMessage = "PAGE_INDEX_MUST_BE_POSITIVE")]
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// The page size used for fetching data.
        /// </summary>
        //[Range(1, MAX_PAGESIZE, ErrorMessage = "PAGE_SIZE_OUT_OF_BOUNDS")]
        public int PageSize { get; set; } = DEFAULT_PAGESIZE;

        /// <summary>
        /// The expression used for sorting.
        /// </summary>
        public string OrderBy { get; set; } = "id";

        /// <summary>
        /// The expression used for filtering the results.
        /// </summary>
        public string Filter { get; set; }

        private const int DEFAULT_PAGESIZE = 20;
        private const int MAX_PAGESIZE = 100;

        public void ThrowOrderByIncorrectException(Exception innerException)
        {
            throw new InputValidationException(innerException,
                (
                    PropertyName: nameof(OrderBy),
                    ErrorMessage: $"The specified orderBy string '{OrderBy}' is invalid."
                )
            );
        }

        public void ThrowFilterIncorrectException(Exception innerException)
        {
            throw new InputValidationException(innerException,
                (
                    PropertyName: nameof(Filter),
                    ErrorMessage: $"The specified filter string '{Filter}' is invalid."
                )
            );
        }
    }
}