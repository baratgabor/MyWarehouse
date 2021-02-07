using FluentAssertions;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Application.Partners.DeletePartner;
using MyWarehouse.Domain.Partners;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MyWarehouse.Application.IntegrationTests.Partners
{
    public class DeletePartnerTests : TestBase
    {
        [Test]
        public void WhenIdIsInvalid_ShouldThrow_EntityNotFoundException()
        {
            var command = new DeletePartnerCommand() { Id = 1234567 };

            FluentActions.Invoking(() => TestFramework.SendAsync(command))
                .Should().Throw<EntityNotFoundException>();
        }

        [Test]
        public async Task WhenIdIsValid_ShouldDeletePartner()
        {
            var createdPartners = await TestFramework.DataFactory.AddPartners(2);

            // Act.
            await TestFramework.SendAsync(new DeletePartnerCommand()
            {
                Id = createdPartners[0].Id
            });

            var deletedPartner = await TestFramework.Data.FindAsync<Partner>(createdPartners[0].Id);
            var anotherPartner = await TestFramework.Data.FindAsync<Partner>(createdPartners[1].Id);
            var allPartners = await TestFramework.Data.GetAllAsync<Partner>();

            deletedPartner.Should().BeNull();
            anotherPartner.Should().NotBeNull();
            allPartners.Should().ContainSingle()
                .And.HaveElementAt(0, anotherPartner);
        }
    }
}
