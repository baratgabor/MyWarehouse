using MyWarehouse.Application.Partners.GetPartnerDetails;
using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Domain.Partners;
using System.Linq;

namespace MyWarehouse.Core.IntegrationTests.Application.Partners
{
    public class GetPartnerDetailsTests : TestBase
    {
        [Test]
        public void WhenPartnerDoesntExist_ShouldThrow_EntityNotFoundException()
        {
            var query = new GetPartnerDetailsQuery() { Id = 45938 };

            FluentActions.Invoking(() => TestFramework.SendAsync(query))
                .Should().Throw<EntityNotFoundException>();
        }

        [Test]
        public async Task WhenPartnerExist_ShouldReturnDetails()
        {
            var partner = (await TestFramework.DataFactory.AddPartners(1)).First();

            // Act.
            var result = await TestFramework.SendAsync(new GetPartnerDetailsQuery() { Id = partner.Id });

            result.Should().NotBeNull();
            result.Id.Should().Be(partner.Id);
            result.Name.Should().Be(partner.Name);

            result.Address.Should().NotBeNull();
            result.Address.Should().BeEquivalentTo(partner.Address, 
                o => o.ComparingByMembers<Address>());
        }
    }
}
