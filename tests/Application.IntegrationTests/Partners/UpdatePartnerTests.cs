using FluentAssertions;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Application.Partners.UpdatePartner;
using MyWarehouse.Domain.Partners;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using static MyWarehouse.Application.Partners.UpdatePartner.UpdatePartnerCommand;

namespace MyWarehouse.Application.IntegrationTests.Partners
{
    public class UpdatePartnerTests : TestBase
    {
        [Test]
        public void WhenDtoIsBlank_ShouldThrow_InputValidationException()
        {
            FluentActions.Invoking(() => TestFramework.SendAsync(new UpdatePartnerCommand()))
                .Should().ThrowExactlyAsync<InputValidationException>();
        }

        [Test]
        public async Task WhenDtoIsValid_ShouldUpdatePartner()
        {
            var partner = (await TestFramework.DataFactory.AddPartners(1)).First();
            var command = new UpdatePartnerCommand()
            {
                Id = partner.Id,
                Name = "New PartnerName",
                Address = new AddressDto()
                {
                    City = "Berlin",
                    Country = "Germany",
                    Street = "Varnhagenstraße 18-44",
                    ZipCode = "10439"
                }
            };

            await TestFramework.SendAsync(command);

            var updatedPartner = await TestFramework.Data.FindAsync<Partner>(command.Id);
            updatedPartner.Should().NotBeNull();
            updatedPartner.Id.Should().Be(command.Id);
            updatedPartner.Name.Should().Be(command.Name);
            updatedPartner.Address.Should().NotBeNull()
                .And.Subject.Should().BeEquivalentTo(command.Address, o => o.ComparingByMembers<AddressDto>());
        }
    }
}
