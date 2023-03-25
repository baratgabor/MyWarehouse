using MyWarehouse.Application.Common.Exceptions;

namespace MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;

public class ListQueryModel<TDto> : IRequest<IListResponseModel<TDto>>
{
    /// <summary>
    /// The index of the page to fetch.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "The minimum page index is 1.")]
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// The page size used for fetching data.
    /// </summary>
    [Range(1, MAX_PAGESIZE)]
    public int PageSize { get; set; } = DEFAULT_PAGESIZE;

    /// <summary>
    /// The expression used for sorting.
    /// </summary>
    public string OrderBy { get; set; } = "id";

    /// <summary>
    /// The expression used for filtering the results.
    /// </summary>
    public string? Filter { get; set; }

    private const int DEFAULT_PAGESIZE = 20;
    private const int MAX_PAGESIZE = 100;

    public void ThrowOrderByIncorrectException(Exception? innerException)
    {
        throw new InputValidationException(innerException,
            (
                PropertyName: nameof(OrderBy),
                ErrorMessage: $"The specified orderBy string '{OrderBy}' is invalid."
            )
        );
    }

    public void ThrowFilterIncorrectException(Exception? innerException)
    {
        throw new InputValidationException(innerException,
            (
                PropertyName: nameof(Filter),
                ErrorMessage: $"The specified filter string '{Filter}' is invalid."
            )
        );
    }
}
