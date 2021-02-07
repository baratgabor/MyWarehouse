using AutoMapper;
using MyWarehouse.Application.Common.Mapping;
using MyWarehouse.Domain.Partners;

namespace MyWarehouse.Application.Partners.GetPartners
{
    public record PartnerDto : IMapFrom<Partner>
    {
        public int Id { get; init; }
        public string Name { get; init; }

        public string Address { get; init; }

        public string Country { get; init; }
        public string ZipCode { get; init; }
        public string City { get; init; }
        public string Street { get; init; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Partner, PartnerDto>()
                .ForMember(dest => dest.Country, x => x.MapFrom(src => src.Address.Country))
                .ForMember(dest => dest.ZipCode, x => x.MapFrom(src => src.Address.ZipCode))
                .ForMember(dest => dest.City, x => x.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.Street, x => x.MapFrom(src => src.Address.Street));
        }
    }
}