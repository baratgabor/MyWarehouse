using MediatR;
using MyWarehouse.Application.Dependencies.Services;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Products.GetProductsSummary
{
    public record ProductStockCountQuery : IRequest<ProductStockCountDto>
    {
    }

    public class ProductStockCountQueryHandler : IRequestHandler<ProductStockCountQuery, ProductStockCountDto>
    {
        private readonly IStockStatisticsService _statisticsService;

        public ProductStockCountQueryHandler(IStockStatisticsService statisticsService)
            => _statisticsService = statisticsService;

        public async Task<ProductStockCountDto> Handle(ProductStockCountQuery request, CancellationToken cancellationToken)
        {
            var res = await _statisticsService.GetProductStockCounts();

            return new ProductStockCountDto() {
                ProductCount = res.ProductCount,
                TotalStock = res.TotalStock
            };
        }
    }
}
