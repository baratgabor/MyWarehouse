using AutoMapper;
using MyWarehouse.Application.Common.Mapping;
using MyWarehouse.Domain.Partners;
using System;

namespace MyWarehouse.Application.Partners.GetPartnerDetails
{
    public record PartnerDetailsDto : IMapFrom<Partner>
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? LastModifiedAt { get; init; }

        public AddressDto Address { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Partner, PartnerDetailsDto>();
            profile.CreateMap<Address, AddressDto>();
        }

        public record AddressDto
        {
            public string Country { get; init; }
            public string ZipCode { get; init; }
            public string City { get; init; }
            public string Street { get; init; }
        }
    }
}
