using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Domain.Products;

namespace MyWarehouse.Application.Products.GetProduct;

public record GetProductDetailsQuery : IRequest<ProductDetailsDto>
{
    public int Id { get; set; }
}

public class GetProductDetailsQueryHandler : IRequestHandler<GetProductDetailsQuery, ProductDetailsDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDetailsDto> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id)
            ?? throw new EntityNotFoundException(nameof(Product), request.Id);

        return _mapper.Map<ProductDetailsDto>(product);
    }
}
