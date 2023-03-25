using FluentAssertions;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Application.Partners.CreatePartner;
using MyWarehouse.Domain.Partners;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using static MyWarehouse.Application.Partners.CreatePartner.CreatePartnerCommand;

namespace MyWarehouse.Application.IntegrationTests.Partners
{
    public class CreatePartnerTests : TestBase
    {
        [Test]
        public void WhenDtoIsBlank_ShouldThrow_InputValidationException()
        {
            var command = new CreatePartnerCommand();

            FluentActions.Invoking(() => TestFramework.SendAsync(command))
                .Should().ThrowAsync<InputValidationException>();
        }

        [Test]
        public async Task WhenDtoIsValid_ShouldCreatePartner()
        {
            var command = new CreatePartnerCommand()
            {
                Address = new AddressDto()
                {
                    City = "Toronto",
                    Country = "Canada",
                    Street = "Street 123",
                    ZipCode = "1234XX"
                },
                Name = "Test Name"
            };

            // Act.
            var partnerId = await TestFramework.SendAsync(command);

            var partner = await TestFramework.Data.FindAsync<Partner>(partnerId, x => x.Transactions);
            partner.Should().NotBeNull();
            partner.Id.Should().Be(partnerId);
            partner.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            partner.CreatedBy.Should().NotBeNullOrWhiteSpace();
            partner.LastModifiedAt.Should().BeNull();
            partner.DeletedAt.Should().BeNull();
            partner.Transactions.Should().BeEmpty();
            partner.Name.Should().Be(command.Name);
            partner.Address.Should().NotBeNull();
            partner.Address.Should().BeEquivalentTo(command.Address, 
                o => o.ComparingByMembers<AddressDto>());
        }
    }
}
