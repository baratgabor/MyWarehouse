using MyWarehouse.Application.Dependencies.Services;
using MyWarehouse.Domain.Common.ValueObjects.Mass;

namespace MyWarehouse.Application.Products.ProductStockMass;

public record ProductStockMassQuery : IRequest<StockMassDto>
{
}

public class ProductStockMassQueryHandler : IRequestHandler<ProductStockMassQuery, StockMassDto>
{
    private readonly IStockStatisticsService _statisticsService;

    public ProductStockMassQueryHandler(IStockStatisticsService statisticsService)
        => _statisticsService = statisticsService;

    public async Task<StockMassDto> Handle(ProductStockMassQuery request, CancellationToken cancellationToken)
    {
        var mass = await _statisticsService.GetProductStockTotalMass(MassUnit.Tonne);

        return new StockMassDto()
        {
            Value = mass.Value,
            Unit = mass.Unit.Symbol
        };
    }
}
