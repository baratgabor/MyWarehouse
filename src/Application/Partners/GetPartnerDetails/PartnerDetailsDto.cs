using MyWarehouse.Application.Common.Mapping;
using MyWarehouse.Domain.Partners;

namespace MyWarehouse.Application.Partners.GetPartnerDetails;

public record PartnerDetailsDto : IMapFrom<Partner>
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }

    public AddressDto Address { get; init; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Partner, PartnerDetailsDto>();
        profile.CreateMap<Address, AddressDto>();
    }

    public record AddressDto
    {
        public string Country { get; init; } = null!;
        public string ZipCode { get; init; } = null!;
        public string City { get; init; } = null!;
        public string Street { get; init; } = null!;
    }
}
