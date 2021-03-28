using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyWarehouse.Application.Products.CreateProduct;
using MyWarehouse.Application.Products.DeleteProduct;
using MyWarehouse.Application.Products.GetProduct;
using MyWarehouse.Application.Products.GetProducts;
using MyWarehouse.Application.Products.GetProductsSummary;
using MyWarehouse.Application.Products.ProductStockMass;
using MyWarehouse.Application.Products.ProductStockValue;
using MyWarehouse.Application.Products.UpdateProduct;
using MyWarehouse.Infrastructure.ApplicationDependencies.DataAccess.Repositories.Common;
using MyWarehouse.WebApi.API.V1;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.WebApi.UnitTests.API.V1
{
    // Simple happy path tests.
    public class ProductControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private ProductController _sut;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>(MockBehavior.Strict);
            _sut = new ProductController(_mockMediator.Object);
        }

        [Test]
        public async Task Create()
        {
            var command = new CreateProductCommand();
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(123);

            var result = await _sut.Create(command);

            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.AreEqual(123, ((OkObjectResult)result.Result).Value);
        }

        [Test]
        public async Task Get()
        {
            var expectedId = 123;
            var response = new ProductDetailsDto();
            _mockMediator.Setup(x => x.Send(It.Is<GetProductDetailsQuery>(q => q.Id == expectedId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.Get(expectedId);

            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.AreEqual(response, ((OkObjectResult)result.Result).Value);
        }

        [Test]
        public async Task GetList()
        {
            var query = new GetProductsListQuery();
            var response = new ListResponseModel<ProductDto>(query, 100, new List<ProductDto>());
            _mockMediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.GetList(query);

            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.AreEqual(response, ((OkObjectResult)result.Result).Value);
        }

        [Test]
        public async Task Update()
        {
            var command = new UpdateProductCommand() { Id = 123 };
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            var result = await _sut.Update(command.Id, command);

            _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Test]
        public async Task Update_WithIdMismatch_ShouldReturnBadRequest()
        {
            var command = new UpdateProductCommand() { Id = 123 };
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            var result = await _sut.Update(456, command);

            Assert.IsAssignableFrom(typeof(BadRequestResult), result);
        }

        [Test]
        public async Task Delete()
        {
            var expectedId = 123;
            _mockMediator.Setup(x => x.Send(It.Is<DeleteProductCommand>(q => q.Id == expectedId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var result = await _sut.Delete(expectedId);

            _mockMediator.Verify(x => x.Send(It.Is<DeleteProductCommand>(c => c.Id == expectedId), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task ProductStockCount()
        {
            var response = new ProductStockCountDto();
            _mockMediator.Setup(x => x.Send(It.IsAny<ProductStockCountQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.ProductStockCount();

            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.AreEqual(response, ((OkObjectResult)result.Result).Value);
        }

        [Test]
        public async Task ProductStockMass()
        {
            var response = new StockMassDto();
            _mockMediator.Setup(x => x.Send(It.IsAny<ProductStockMassQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.ProductStockMass();

            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.AreEqual(response, ((OkObjectResult)result.Result).Value);
        }

        [Test]
        public async Task ProductStockValue()
        {
            var response = new StockValueDto();
            _mockMediator.Setup(x => x.Send(It.IsAny<ProductStockValueQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.ProductStockValue();

            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.AreEqual(response, ((OkObjectResult)result.Result).Value);
        }
    }
}