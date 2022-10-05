using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Domain.Common.ValueObjects.Mass;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Domain.Products;

namespace MyWarehouse.Application.Products.CreateProduct;

public record CreateProductCommand : IRequest<int>
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;

    public float MassValue { get; init; }
    public string MassUnitSymbol { get; init; } = null!;

    public decimal PriceAmount { get; init; }
    public string PriceCurrencyCode { get; init; } = null!;
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            name: request.Name.Trim(),
            description: request.Description.Trim(),
            price: new Money(request.PriceAmount, ProductInvariants.DefaultPriceCurrency),
            mass: new Mass(request.MassValue, ProductInvariants.DefaultMassUnit)
        );

        _unitOfWork.Products.Add(product);
        await _unitOfWork.SaveChanges();

        return product.Id;
    }
}
