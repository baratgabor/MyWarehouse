using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;
using MyWarehouse.Application.Partners.CreatePartner;
using MyWarehouse.Application.Partners.DeletePartner;
using MyWarehouse.Application.Partners.GetPartnerDetails;
using MyWarehouse.Application.Partners.GetPartners;
using MyWarehouse.Application.Partners.UpdatePartner;
using MyWarehouse.Infrastructure.ApplicationDependencies.DataAccess.Repositories.Common;
using MyWarehouse.WebApi.API.V1;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.WebApi.UnitTests.Controllers
{
    // Simple happy path tests.
    public class PartnerControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private PartnerController _sut;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>(MockBehavior.Strict);
            _sut = new PartnerController(_mockMediator.Object);
        }

        [Test]
        public async Task Create()
        {
            var command = new CreatePartnerCommand();
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
            var response = new PartnerDetailsDto();
            _mockMediator.Setup(x => x.Send(It.Is<GetPartnerDetailsQuery>(q => q.Id == expectedId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.Get(expectedId);

            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.AreEqual(response, ((OkObjectResult)result.Result).Value);
        }

        [Test]
        public async Task GetList()
        {
            var query = new ListQueryModel<PartnerDto>();
            var response = new ListResponseModel<PartnerDto>(query, 100, new List<PartnerDto>());
            _mockMediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _sut.GetList(query);

            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.AreEqual(response, ((OkObjectResult)result.Result).Value);
        }

        [Test]
        public async Task Update()
        {
            var command = new UpdatePartnerCommand() { Id = 123 };
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            var result = await _sut.Update(command.Id, command);

            _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Update_WithIdMismatch_ShouldReturnBadRequest()
        {
            var command = new UpdatePartnerCommand() { Id = 123 };
            _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            var result = await _sut.Update(456, command);

            Assert.IsAssignableFrom(typeof(BadRequestResult), result);
        }

        [Test]
        public async Task Delete()
        {
            var expectedId = 123;
            _mockMediator.Setup(x => x.Send(It.Is<DeletePartnerCommand>(q => q.Id == expectedId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var result = await _sut.Delete(expectedId);

            _mockMediator.Verify(x => x.Send(It.Is<DeletePartnerCommand>(c => c.Id == expectedId), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}