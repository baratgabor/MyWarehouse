using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;

namespace MyWarehouse.Application.Products.GetProducts;

public class GetProductsListQuery : ListQueryModel<ProductDto>
{
    public ProductStatus Status { get; init; }
    public bool StockedOnly => Status == ProductStatus.Stocked;

    public enum ProductStatus
    {
        Default,
        Stocked
    }
}

public class GetProductsListQueryHandler : IRequestHandler<GetProductsListQuery, IListResponseModel<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsListQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public Task<IListResponseModel<ProductDto>> Handle(GetProductsListQuery request, CancellationToken cancellationToken)
        => _unitOfWork.Products.GetProjectedListAsync(request,
            additionalFilter: request.StockedOnly ? x => x.NumberInStock > 0 : null,
            readOnly: true);
}
