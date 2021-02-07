using MediatR;
using MyWarehouse.Application.Dependencies.Services;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Products.ProductStockValue
{
    public record ProductStockValueQuery : IRequest<StockValueDto>
    {
    }

    public class ProductStockValueQueryHandler : IRequestHandler<ProductStockValueQuery, StockValueDto>
    {
        private readonly IStockStatisticsService _statisticsService;

        public ProductStockValueQueryHandler(IStockStatisticsService stockStatisticsService)
            => _statisticsService = stockStatisticsService;

        public async Task<StockValueDto> Handle(ProductStockValueQuery request, CancellationToken cancellationToken)
        {
            var totalStockValue = await _statisticsService.GetProductStockTotalValue();

            return new StockValueDto()
            {
                Amount = totalStockValue.Amount,
                CurrencyCode = totalStockValue.Currency.Code
            };
        }
    }
}
