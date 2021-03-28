using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyWarehouse.Application.Transactions.CreateTransaction;
using MyWarehouse.Application.Transactions.GetTransactionDetails;
using MyWarehouse.Application.Transactions.GetTransactionsList;
using MyWarehouse.Infrastructure.ApplicationDependencies.DataAccess.Repositories.Common;
using MyWarehouse.WebApi.API.V1;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.WebApi.UnitTests.API.V1
{
    // Simple happy path tests.
    public class TransactionControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private TransactionController _sut;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>(MockBehavior.Strict);
            _sut = new TransactionController(_mockMediator.Object);
        }

        [Test]
        public async Task Create()
        {
            var command = new CreateTransactionCommand();
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
            var response = new TransactionDetailsDto();
            _mockMediator.Setup(x => x.Send(It.Is<GetTransactionDetailsQuery>(q => q.Id == expectedId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.Get(expectedId);

            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.AreEqual(response, ((OkObjectResult)result.Result).Value);
        }

        [Test]
        public async Task GetList()
        {
            var query = new GetTransactionListQuery();
            var response = new ListResponseModel<TransactionDto>(query, 100, new List<TransactionDto>());
            _mockMediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.GetList(query);

            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.AreEqual(response, ((OkObjectResult)result.Result).Value);
        }
    }
}