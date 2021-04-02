using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;
using MyWarehouse.Application.Products.CreateProduct;
using MyWarehouse.Application.Products.DeleteProduct;
using MyWarehouse.Application.Products.GetProduct;
using MyWarehouse.Application.Products.GetProducts;
using MyWarehouse.Application.Products.GetProductsSummary;
using MyWarehouse.Application.Products.ProductStockMass;
using MyWarehouse.Application.Products.ProductStockValue;
using MyWarehouse.Application.Products.UpdateProduct;
using System.Threading.Tasks;

namespace MyWarehouse.WebApi.API.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{v:apiVersion}/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateProductCommand command)
            => Ok(await _mediator.Send(command));

        [HttpGet]
        public async Task<ActionResult<IListResponseModel<ProductDto>>> GetList([FromQuery] GetProductsListQuery query)
            => Ok(await _mediator.Send(query));

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailsDto>> Get(int id)
            => Ok(await _mediator.Send(new GetProductDetailsQuery() { Id = id }));

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteProductCommand() { Id = id });

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateProductCommand command)
        {
            if (id != command.Id) return BadRequest();

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpGet("totalMass")]
        public async Task<ActionResult<StockMassDto>> ProductStockMass()
            => Ok(await _mediator.Send(new ProductStockMassQuery()));

        [HttpGet("totalValue")]
        public async Task<ActionResult<StockValueDto>> ProductStockValue()
            => Ok(await _mediator.Send(new ProductStockValueQuery()));

        [HttpGet("stockCount")]
        public async Task<ActionResult<ProductStockCountDto>> ProductStockCount()
            => Ok(await _mediator.Send(new ProductStockCountQuery()));
    }
}
