using FluentAssertions;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;
using MyWarehouse.Application.Partners.GetPartners;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MyWarehouse.Core.IntegrationTests.Application.Partners
{
    public class GetPartnersListTests : TestBase
    {
        [Test]
        public async Task WhenEmpty_ShouldReturnEmptyList()
        {
            var list = await TestFramework.SendAsync(new ListQueryModel<PartnerDto>());

            list.Should().NotBeNull();
            list.RowCount.Should().Be(0);
            list.Results.Should().BeEmpty();
        }

        [Test]
        public async Task WhenPartnersExist_ShouldReturnPagedResults ()
        {
            await  TestFramework.DataFactory.AddPartners(31);

            // Act.
            var list = await TestFramework.SendAsync(new ListQueryModel<PartnerDto>());

            list.RowCount.Should().Be(31);
            list.PageIndex.Should().Be(1);
            list.Results.Should().NotBeNull()
                .And.HaveCount(list.PageSize)
                .And.NotContainNulls()
                .And.OnlyHaveUniqueItems();
        }
    }
}
